# Assembles a .NET SDK store root with the Android workload baked in, from the
# hash-pinned NuGet packages listed in android-workload-packs.nix (generated
# by gen-android-workload.sh).
{
  lib,
  stdenvNoCC,
  fetchurl,
  fetchNupkg,
  makeWrapper,
  dotnet-sdk,
  lock,
}: let
  manifestDirId = p: lib.removeSuffix ".manifest-${lock.manifestBand}" p.id;
  unpacked = p: "${p.pkg}/share/nuget/packages/${lib.toLower p.id}/${lib.toLower p.version}";

  fetchPkg = p:
    p
    // {
      pkg = fetchNupkg {
        pname = p.id;
        inherit (p) version url hash;
      };
    };

  manifests = map fetchPkg lock.manifests;
  packs = map fetchPkg lock.packs;

  rawTemplatePacks = map (p:
    p
    // {
      src = fetchurl {
        name = "nupkg-${p.id}-${p.version}";
        inherit (p) url hash;
      };
    })
  lock.templatePacks;
in
  stdenvNoCC.mkDerivation {
    pname = "dotnet-sdk-with-android-workload";
    version = lock.workloadVersion;

    nativeBuildInputs = [makeWrapper];

    dontUnpack = true;
    dontPatch = true;
    dontConfigure = true;
    dontFixup = true;

    installPhase = ''
      runHook preInstall

      mkdir -p "$out"
      cp -R ${dotnet-sdk}/share/dotnet/. "$out/"
      chmod -R u+w "$out"
      rm -rf "$out/metadata" "$out/bin"

      ${lib.concatMapStringsSep "\n" (m: ''
          dest="$out/sdk-manifests/${lock.manifestBand}/${manifestDirId m}/${m.version}"
          rm -rf "$out/sdk-manifests/${lock.manifestBand}/${manifestDirId m}"
          mkdir -p "$dest"
          cp -r ${unpacked m}/data/. "$dest/"
        '')
        manifests}

      ${lib.concatMapStringsSep "\n" (p: ''
          dest="$out/packs/${p.id}/${p.version}"
          mkdir -p "$dest"
          cp -r ${unpacked p}/. "$dest/"
          chmod -R u+w "$dest"
        '')
        packs}

      sdkTools="$out/packs/Microsoft.Android.Sdk.Linux/${lock.workloadVersion}/tools"
      cat > "$sdkTools/NixStorePermissions.targets" <<'EOF'
      <Project>
        <Target Name="_NixMakeIntermediateWritable" AfterTargets="_LinkAssembliesNoShrink" Condition="'$(_NixMakeIntermediateWritableDone)' != 'true'">
          <PropertyGroup>
            <_NixMakeIntermediateWritableDone>true</_NixMakeIntermediateWritableDone>
          </PropertyGroup>
          <Exec Command="chmod -R u+w &quot;$(IntermediateOutputPath)&quot;" Condition="Exists('$(IntermediateOutputPath)')" />
        </Target>
        <Target Name="_NixMakeIntermediateWritableILLink" AfterTargets="_AfterILLinkAdditionalSteps" Condition="'$(_NixMakeIntermediateWritableDone)' != 'true'">
          <PropertyGroup>
            <_NixMakeIntermediateWritableDone>true</_NixMakeIntermediateWritableDone>
          </PropertyGroup>
          <Exec Command="chmod -R u+w &quot;$(IntermediateOutputPath)&quot;" Condition="Exists('$(IntermediateOutputPath)')" />
        </Target>
      </Project>
      EOF
      afterTargets="$out/packs/Microsoft.Android.Sdk.Linux/${lock.workloadVersion}/targets/Microsoft.Android.Sdk.After.targets"
      sed -i 's|^</Project>\r\?$|  <Import Project="..\\tools\\NixStorePermissions.targets" />\r\n</Project>|' "$afterTargets"

      mkdir -p "$out/template-packs"
      ${lib.concatMapStringsSep "\n" (p: ''
          cp ${p.src} "$out/template-packs/${p.id}.${p.version}.nupkg"
        '')
        rawTemplatePacks}

      recordsRoot="$out/metadata/workloads"
      mkdir -p "$recordsRoot/${lock.sdkBand}/InstalledWorkloads"
      touch "$recordsRoot/${lock.sdkBand}/InstalledWorkloads/android"
      ${lib.concatMapStringsSep "\n" (m: ''
          dir="$recordsRoot/InstalledManifests/v1/${manifestDirId m}/${m.version}/${lock.manifestBand}"
          mkdir -p "$dir"
          touch "$dir/${lock.sdkBand}"
        '')
        manifests}
      ${lib.concatMapStringsSep "\n" (p: ''
          dir="$recordsRoot/InstalledPacks/v1/${p.id}/${p.version}"
          mkdir -p "$dir"
          touch "$dir/${lock.sdkBand}"
        '')
        (packs ++ rawTemplatePacks)}

      mkdir -p "$out/bin"
      makeWrapper "$out/dotnet" "$out/bin/dotnet" \
        --set DOTNET_ROOT "$out"

      runHook postInstall
    '';
  }
