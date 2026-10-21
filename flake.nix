{
  description = ''MCalc: dev environment with the .NET Android workload baked into a store SDK root. '';

  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/nixos-unstable";
    android-nixpkgs.url = "github:tadfisher/android-nixpkgs/stable";
    flake-utils.url = "github:numtide/flake-utils";
  };

  outputs = {
    self,
    nixpkgs,
    android-nixpkgs,
    flake-utils,
  }: let
    sdkPin = import ./nix/sdk-pin.nix;
    lock = import ./nix/android-workload-packs.nix;

    mkToolchain = pkgs: let
      inherit (pkgs) lib;

      # Immutable Android SDK from android-nixpkgs.
      sdkPackageAttrs = [
        "build-tools-36-0-0"
        "cmdline-tools-19-0"
        "platform-tools"
        "platforms-android-36"
      ];

      androidSdk =
        android-nixpkgs.sdk.${pkgs.stdenv.hostPlatform.system}
        (sdkPkgs:
          lib.attrValues (
            lib.getAttrs sdkPackageAttrs sdkPkgs
          ));

      androidSdkRoot = "${androidSdk}/share/android-sdk";

      # .NET SDK with the Android workload baked into its store root.
      dotnetSdk = import ./nix/dotnet-android-sdk.nix {
        inherit (pkgs) lib stdenvNoCC fetchurl makeWrapper;
        inherit (pkgs.dotnetCorePackages) fetchNupkg;

        dotnet-sdk =
          pkgs.dotnetCorePackages."sdk_${sdkPin.major}_0";

        inherit lock;
      };

      jdk = pkgs.jdk17_headless;

      # Map Android SDK requirement names to android-nixpkgs attributes.
      requirementToAttr = req: let
        parts = lib.splitString ";" req;
      in
        if builtins.head parts == "platforms"
        then "platforms-${lib.concatStringsSep "-" (builtins.tail parts)}"
        else
          lib.replaceStrings ["."] ["-"]
          (lib.concatStringsSep "-" parts);

      unmetRequirements =
        lib.filter
        (req: !(builtins.elem (requirementToAttr req) sdkPackageAttrs))
        lock.requiredAndroidSdkPackages;
    in
      assert lib.assertMsg (unmetRequirements == []) ''
        MCalc: the workload lock requires Android SDK packages that
        android-nixpkgs does not provide here:
        ${lib.concatStringsSep ", " unmetRequirements}.

        Update sdkPackageAttrs in flake.nix and
        android-workload-packs.nix together.
        Regenerate with: nix run .#update-workload
      ''; {
        inherit androidSdk androidSdkRoot dotnetSdk jdk lock;
      };
  in
    flake-utils.lib.eachSystem ["x86_64-linux"] (
      system: let
        pkgs = import nixpkgs {
          inherit system;
        };

        toolchain = mkToolchain pkgs;

        app = name: path: let
          script = pkgs.writeShellApplication {
            name = "mcalc-${name}";

            runtimeInputs = [
              pkgs.bash
              pkgs.coreutils
              pkgs.curl
              pkgs.findutils
              pkgs.gnugrep
              pkgs.gnused
              pkgs.jq
            ];

            text = ''
              export MCALC_LIB="${./nix/lib.sh}"
              exec bash ${path} "$@"
            '';
          };
        in {
          type = "app";
          program = "${script}/bin/mcalc-${name}";
        };
      in {
        packages = {
          dotnet-base =
            pkgs.dotnetCorePackages."sdk_${sdkPin.major}_0";

          dotnet-sdk-with-workload = toolchain.dotnetSdk;

          # Allows `nix build` to build the customized SDK.
          default = toolchain.dotnetSdk;
        };

        apps = {
          update-workload =
            app "update-workload" ./nix/update-workload.sh;

          update-sdk =
            app "update-sdk" ./nix/update-sdk.sh;

          upgrade-sdk =
            app "upgrade-sdk" ./nix/upgrade-sdk.sh;

          status =
            app "status" ./nix/status.sh;
        };

        devShells.default = pkgs.mkShell {
          name = "mcalc";

          packages = [
            toolchain.dotnetSdk
            toolchain.androidSdk
            toolchain.jdk
            pkgs.git
          ];

          env = {
            DOTNET_ROOT = "${toolchain.dotnetSdk}";
            ANDROID_HOME = toolchain.androidSdkRoot;
            ANDROID_SDK_ROOT = toolchain.androidSdkRoot;
            JAVA_HOME = "${toolchain.jdk}";
          };

          shellHook = ''
            export NUGET_PACKAGES="$PWD/.nuget/packages"
          '';
        };
      }
    );
}
