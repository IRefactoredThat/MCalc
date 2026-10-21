#!/usr/bin/env bash
# Regenerates android-workload-packs.nix, the fixed-output lock backing the
# store-packaged .NET Android workload.
# Run from repo root: ./nix/gen-android-workload.sh.

set -euo pipefail

REPO_ROOT="$(cd "$(dirname "$0")/.." && pwd)"
PROJECT="${PROJECT:-CalculatorApp/CalculatorApp.csproj}"
NIXPKGS_FLAKE="${NIXPKGS_FLAKE:-$REPO_ROOT}"

if [ ! -f "$REPO_ROOT/$PROJECT" ] && [ ! -f "$PROJECT" ]; then
  echo "error: could not find project $PROJECT (cwd=$(pwd))" >&2
  exit 1
fi
if [ -f "$REPO_ROOT/$PROJECT" ]; then
  PROJECT_ABS="$REPO_ROOT/$PROJECT"
else
  PROJECT_ABS="$PROJECT"
fi

need_bootstrap=0
if [ "${FORCE_BOOTSTRAP:-0}" = "1" ] || [ -z "${DOTNET_STATE:-}" ]; then
  need_bootstrap=1
fi
DOTNET_STATE="${DOTNET_STATE:-}"
if [ $need_bootstrap -eq 0 ]; then
  if [ ! -d "$DOTNET_STATE/sdk-manifests" ] || [ -z "$(ls -A "$DOTNET_STATE/sdk-manifests" 2>/dev/null)" ]; then
    need_bootstrap=1
  fi
fi

TMPDIR_STATE=""
DOWNLOAD_DIR="$(mktemp -d /tmp/mcalc-nupkg.XXXXXX)"
cleanup() {
  if [ -n "${TMPDIR_STATE:-}" ] && [ -d "$TMPDIR_STATE" ]; then
    rm -rf "$TMPDIR_STATE"
  fi
  if [ -n "${DOWNLOAD_DIR:-}" ] && [ -d "$DOWNLOAD_DIR" ]; then
    rm -rf "$DOWNLOAD_DIR"
  fi
}
trap cleanup EXIT

# The workload-manifest band of the SDK in use: 10.0.100 for the whole 10.0.x
# line, 11.0.100-rc.1 for a preview SDK. It is not derivable from the SDK
# version, so it is read back from the state the installer wrote — the
# installer records every installed manifest under
# metadata/workloads/InstalledManifests/v1/<id>/<version>/<manifest band>/.
detect_manifest_band() {
  local installed="$DOTNET_STATE/metadata/workloads/InstalledManifests/v1/microsoft.net.sdk.android"
  local band
  band="$(find "$installed" -mindepth 2 -maxdepth 2 -type d 2>/dev/null | sort -V | tail -n1 || true)"
  if [ -n "$band" ]; then
    basename "$band"
    return 0
  fi
  # Fall back to the state layout: the band directory holding the manifests.
  local d
  for d in "$DOTNET_STATE"/sdk-manifests/*/; do
    [ -d "$d/microsoft.net.sdk.android" ] || continue
    basename "$d"
  done | sort -V | tail -n1
}

# The SDK feature band (10.0.400), recorded by the installer under
# metadata/workloads/<arch>/<band>. The arch directory is X64 on x86_64 and
# Arm64 on aarch64, so it is matched by shape instead of by name.
sdk_band_for_state() {
  local d
  d="$(find "$DOTNET_STATE/metadata/workloads" -mindepth 2 -maxdepth 2 -type d 2>/dev/null \
    | grep -E '/[0-9]+\.[0-9]+\.[0-9]+' | sort -V | tail -n1 || true)"
  if [ -n "$d" ]; then
    basename "$d"
  else
    echo "unknown"
  fi
}

if [ $need_bootstrap -eq 1 ]; then
  echo "bootstrapping workload state via 'dotnet workload restore' (no usable DOTNET_STATE)..."
  if ! command -v nix >/dev/null 2>&1; then
    echo "error: nix required to bootstrap the plain SDK" >&2
    exit 1
  fi
  TMPDIR_STATE="$(mktemp -d /tmp/mcalc-dotnet-bootstrap.XXXXXX)"
  # The SDK package has several outputs; pick the one carrying the muxer.
  SDK=""
  while read -r candidate; do
    if [ -x "$candidate/bin/dotnet" ]; then
      SDK="$candidate"
      break
    fi
  done < <(nix --accept-flake-config build --no-link --print-out-paths "$NIXPKGS_FLAKE#dotnet-base")
  if [ -z "$SDK" ]; then
    echo "error: failed to locate plain dotnet SDK from $NIXPKGS_FLAKE#dotnet-base" >&2
    exit 1
  fi
  export DOTNET_CLI_HOME="$TMPDIR_STATE"
  export DOTNET_ROOT="$SDK"
  export PATH="$SDK/bin:$PATH"
  export DOTNET_CLI_TELEMETRY_OPTOUT=1
  export DOTNET_NOLOGO=1
  export NUGET_HTTP_CACHE_PATH="$TMPDIR_STATE/nuget/http-cache"
  export NUGET_PACKAGES="$TMPDIR_STATE/nuget/packages"
  mkdir -p "$NUGET_PACKAGES" "$NUGET_HTTP_CACHE_PATH"
  sdk_ver="$(dotnet --version)"
  DOTNET_STATE="$TMPDIR_STATE/.dotnet"
  echo "  SDK: $sdk_ver"
  echo "  restoring: $PROJECT_ABS"
  (cd "$REPO_ROOT" && timeout 1800 dotnet workload restore "$PROJECT_ABS") >/dev/null
fi

# The manifest band follows the SDK in use, so it is read back from the state
# instead of being derived from the SDK version.
if [ -z "${MANIFEST_BAND:-}" ]; then
  MANIFEST_BAND="$(detect_manifest_band)"
  if [ -z "$MANIFEST_BAND" ]; then
    echo "error: could not detect the manifest band from $DOTNET_STATE" >&2
    exit 1
  fi
fi
DOTNET_STATE="${DOTNET_STATE:-$HOME/.dotnet}"
PACKS_DIR="$DOTNET_STATE/packs"
TEMPLATE_PACKS_DIR="$DOTNET_STATE/template-packs"
MANIFESTS_DIR="$DOTNET_STATE/sdk-manifests/$MANIFEST_BAND"
OUT="$REPO_ROOT/nix/android-workload-packs.nix"

MANIFEST_IDS=()
for d in "$MANIFESTS_DIR"/*/; do
  [ -d "$d" ] || continue
  # The workload set is bundled with the SDK, never published as a package;
  # it is not part of the lock.
  [ "$(basename "$d")" = "workloadsets" ] && continue
  MANIFEST_IDS+=("$(basename "$d")")
done

if [ ${#MANIFEST_IDS[@]} -eq 0 ]; then
  echo "error: no manifests found in $MANIFESTS_DIR" >&2
  echo "hint: run ./nix/gen-android-workload.sh from the repo (it will bootstrap), or set DOTNET_STATE" >&2
  exit 1
fi

flat_url() {
  local id="${1,,}" ver="$2"
  echo "https://api.nuget.org/v3-flatcontainer/$id/$ver/$id.$ver.nupkg"
}

hash_pkg() {
  local id="$1" ver="$2" nupkg
  nupkg="$PACKS_DIR/$id/$ver/$id.nupkg"
  if [ -f "$nupkg" ]; then
    nix hash file --sri --type sha512 "$nupkg"
    return
  fi
  # Manifests are not installed as packs, so they are fetched and hashed.
  # The registration API does not serve prerelease packages, the flat
  # container does.
  local download="$DOWNLOAD_DIR/$id.$ver.nupkg"
  curl -fsSL "$(flat_url "$id" "$ver")" -o "$download"
  nix hash file --sri --type sha512 "$download"
}

emit_pkg() {
  local id="$1" ver="$2" hash url
  hash=$(hash_pkg "$id" "$ver") || {
    echo "error: could not determine hash for $id $ver" >&2
    exit 1
  }
  url=$(flat_url "$id" "$ver")
  printf '  {\n    id = "%s";\n    version = "%s";\n    url = "%s";\n    hash = "%s";\n  }\n' "$id" "$ver" "$url" "$hash"
}

workload_version=""
for m in "${MANIFEST_IDS[@]}"; do
  if [ ! -d "$MANIFESTS_DIR/$m" ]; then
    echo "error: manifest $m not installed under $MANIFESTS_DIR" >&2
    exit 1
  fi
done
workload_version=$(basename "$(find "$MANIFESTS_DIR/microsoft.net.sdk.android" -mindepth 1 -maxdepth 1 -type d | sort -V | tail -n1)")

required_sdk_packages=$(jq -r '[to_entries[0].value.androidsdk.packages[] | select(.optional == "false") | .sdkPackage.id] | join(" ")' "$MANIFESTS_DIR/microsoft.net.sdk.android/$workload_version/WorkloadDependencies.json")
sdk_band=$(sdk_band_for_state)

{
  cat <<'LOCKHDR'
# Generated by gen-android-workload.sh — do not edit by hand.
#
# Fixed-output lock for the store-packaged .NET Android workload (see dotnet-android-sdk.nix and the
# devShell in ../flake.nix). Regenerate it from the repo root:
#   nix run .#update-workload    # newest published workload
#   nix run .#update-sdk         # newest SDK patch of the pinned line
#   nix run .#upgrade-sdk 11     # another SDK line
#
# Self-bootstraps via 'dotnet workload restore' using the plain nixpkgs SDK when no usable
# ~/.dotnet state exists. The manifest and SDK bands are read back from the bootstrapped state,
# so they follow the SDK pinned in ../nix/sdk-pin.nix automatically.
{
LOCKHDR
  printf '  manifestBand = "%s";\n' "$MANIFEST_BAND"
  printf '  sdkBand = "%s";\n' "$sdk_band"
  printf '  workloadVersion = "%s";\n' "$workload_version"
  cat <<'LOCKMID'
  # Non-optional Android SDK packages the workload manifest requires; asserted
  # against the android-nixpkgs package set in ../flake.nix.
  requiredAndroidSdkPackages = [
LOCKMID
  for p in $required_sdk_packages; do
    printf '    "%s"\n' "$p"
  done
  cat <<'LOCKMID2'
  ];
  manifests = [
LOCKMID2
  for m in "${MANIFEST_IDS[@]}"; do
    ver=$(basename "$(find "$MANIFESTS_DIR/$m" -mindepth 1 -maxdepth 1 -type d | sort -V | tail -n1)")
    emit_pkg "$m.manifest-$MANIFEST_BAND" "$ver"
  done
  cat <<'LOCKMID3'
  ];
  packs = [
LOCKMID3
  for d in "$PACKS_DIR"/*/*/; do
    [ -d "$d" ] || continue
    ver=$(basename "$d")
    id=$(basename "$(dirname "$d")")
    [ -f "$d/$id.nupkg" ] || continue
    emit_pkg "$id" "$ver"
  done
  cat <<'LOCKMID4'
  ];
  templatePacks = [
LOCKMID4
  if [ -d "$TEMPLATE_PACKS_DIR" ]; then
    for f in "$TEMPLATE_PACKS_DIR"/*.nupkg; do
      [ -e "$f" ] || continue
      base=$(basename "$f" .nupkg)
      if [[ "$base" =~ ^(.*)\.([0-9]+\.[0-9]+\.[0-9]+.*)$ ]]; then
        id="${BASH_REMATCH[1]}"
        ver="${BASH_REMATCH[2]}"
        hash=$(nix hash file --sri --type sha512 "$f")
        printf '  {\n    id = "%s";\n    version = "%s";\n    url = "%s";\n    hash = "%s";\n  }\n' "$id" "$ver" "$(flat_url "$id" "$ver")" "$hash"
      fi
    done
  fi
  cat <<'LOCKFOOT'
  ];
}
LOCKFOOT
} >"$OUT"

# The lock is written by hand-rolled printf calls; nixfmt gives it the same
# shape as the rest of the nix files in the repository.
if formatter="$(nix --accept-flake-config build --no-link --print-out-paths "$NIXPKGS_FLAKE#nixfmt" 2>/dev/null)" \
  && [ -x "$formatter/bin/nixfmt" ]; then
  "$formatter/bin/nixfmt" "$OUT"
else
  echo "warning: nixfmt unavailable, the lock is left unformatted" >&2
fi

echo "wrote $OUT (manifestBand $MANIFEST_BAND, sdkBand $sdk_band, workload $workload_version)"
