# shellcheck shell=bash
# Shared helpers for the toolchain maintenance commands.
#
# Sourced by nix/update-workload.sh, nix/update-sdk.sh, nix/upgrade-sdk.sh and
# nix/status.sh.

PIN_FILE="nix/sdk-pin.nix"
LOCK_FILE="nix/android-workload-packs.nix"
GENERATOR="nix/gen-android-workload.sh"

die() {
  echo "error: $*" >&2
  exit 1
}

step() {
  echo
  echo "==> $*"
}

note() {
  echo "    $*"
}

find_repo_root() {
  local dir="$PWD"
  while [ "$dir" != "/" ]; do
    if [ -f "$dir/flake.nix" ] && [ -f "$dir/$PIN_FILE" ]; then
      printf '%s' "$dir"
      return 0
    fi
    dir="$(dirname "$dir")"
  done
  return 1
}

REPO_ROOT="${MCALC_ROOT:-$(find_repo_root || true)}"
if [ -z "$REPO_ROOT" ]; then
  die "not inside the MCalc repository (no flake.nix found); run this from the repo root or set MCALC_ROOT"
fi
cd "$REPO_ROOT" || exit 1

require_tools() {
  local tool
  for tool in "$@"; do
    command -v "$tool" >/dev/null 2>&1 || die "$tool is required but not on PATH"
  done
}

# nix eval without the "Git tree is dirty".
nix_eval() {
  local out err
  err="$(mktemp)"
  if ! out="$(nix eval "$@" 2>"$err")"; then
    cat "$err" >&2
    rm -f "$err"
    return 1
  fi
  rm -f "$err"
  printf '%s' "$out"
}

current_system() {
  nix_eval --impure --raw --expr 'builtins.currentSystem'
}

# The .NET SDK line the toolchain is pinned to (nix/sdk-pin.nix).
sdk_major() {
  sed -n 's/^[[:space:]]*major[[:space:]]*=[[:space:]]*"\([0-9]\+\)";.*/\1/p' "$PIN_FILE" | head -n1
}

# The version of the SDK the pinned line resolves to in nixpkgs.
sdk_version() {
  nix_eval --raw ".#packages.$(current_system).dotnet-base.version"
}

# The version of the SDK nixpkgs ships for another line.
sdk_version_for() {
  local major="$1" system
  system="$(current_system)"
  nix_eval --impure --raw --expr "
    let flake = builtins.getFlake (toString ./.);
        pkgs = flake.inputs.nixpkgs.legacyPackages.\"$system\";
    in pkgs.dotnetCorePackages.\"sdk_${major}_0\".version"
}

# A field of the generated workload lock.
lock_field() {
  sed -n "s/^[[:space:]]*$1[[:space:]]*=[[:space:]]*\"\\([^\"]*\\)\";.*/\\1/p" "$LOCK_FILE" | head -n1
}

# The android workload manifest the lock pins, as "<id> <version>".
locked_workload_manifest() {
  awk '
    function value(line,   start, len) {
      if (match(line, /"[^"]*"/)) {
        return substr(line, start = RSTART + 1, len = RLENGTH - 2)
      }
      return ""
    }
    /^[[:space:]]*id[[:space:]]*=/ { id = value($0) }
    /^[[:space:]]*version[[:space:]]*=/ {
      if (id ~ /^microsoft\.net\.sdk\.android\.manifest-/) {
        print id " " value($0)
        exit
      }
    }
  ' "$LOCK_FILE"
}

# The newest version of a package published on nuget.org.
latest_published_version() {
  local id="${1,,}"
  curl -fsSL "https://api.nuget.org/v3-flatcontainer/$id/index.json" 2>/dev/null \
    | jq -r '.versions[-1] // empty'
}

# Rewrite the SDK line in the pin file.
set_sdk_major() {
  local major="$1"
  local matches
  matches="$(grep -c '^[[:space:]]*major[[:space:]]*=[[:space:]]*"[0-9]\+";' "$PIN_FILE" || true)"
  [ "$matches" = "1" ] || die "expected exactly one 'major' pin in $PIN_FILE, found $matches"
  sed -i -E "s/^([[:space:]]*major[[:space:]]*=[[:space:]]*\")[0-9]+(\";)/\1$major\2/" "$PIN_FILE"
}

# Regenerate the workload lock.
refresh_workload_lock() {
  step "Refreshing the workload lock ($LOCK_FILE)"
  if [ -n "${DOTNET_STATE:-}" ]; then
    note "using workload state $DOTNET_STATE"
    "$GENERATOR"
  else
    note "bootstrapping a fresh workload state (newest published packs)"
    FORCE_BOOTSTRAP=1 "$GENERATOR"
  fi
}

# Build the store-packaged SDK.
build_toolchain() {
  step "Rebuilding the toolchain"
  nix build ".#dotnet-sdk-with-workload" --no-link
  note "ok"
}

# Restore the pin and the lock when an update fails half-way.
snapshot_state() {
  SNAPSHOT_DIR="$(mktemp -d)"
  cp "$PIN_FILE" "$SNAPSHOT_DIR/sdk-pin.nix"
  cp "$LOCK_FILE" "$SNAPSHOT_DIR/android-workload-packs.nix"
}

restore_state() {
  local status=$?
  if [ -n "${SNAPSHOT_DIR:-}" ] && [ -d "$SNAPSHOT_DIR" ]; then
    if [ $status -ne 0 ]; then
      cp "$SNAPSHOT_DIR/sdk-pin.nix" "$PIN_FILE"
      cp "$SNAPSHOT_DIR/android-workload-packs.nix" "$LOCK_FILE"
      echo "error: the update failed; the SDK pin and the workload lock were restored" >&2
    fi
    rm -rf "$SNAPSHOT_DIR"
    SNAPSHOT_DIR=""
  fi
  return $status
}

print_summary() {
  step "Summary"
  note "SDK line:      .NET $(sdk_major) ($(sdk_version))"
  note "workload:      $(lock_field workloadVersion)"
  note "manifest band: $(lock_field manifestBand)"
  note "SDK band:      $(lock_field sdkBand)"
  echo
  note "review:        git diff $PIN_FILE $LOCK_FILE"
  note "reload shell:  direnv reload   (or: nix develop)"
}

# What a line change still needs outside the nix integration.
print_upgrade_followups() {
  local major="$1" previous="$2"
  local skip=(--exclude-dir=.nuget --exclude-dir=.git --exclude-dir=obj --exclude-dir=bin)
  step "Still to do outside the nix integration"
  note "target frameworks: net$previous.0-android -> net$major.0-android, net$previous.0 -> net$major.0"
  grep -rn "${skip[@]}" --include='*.csproj' -E "net$previous\.0" . | sed 's/^/    /' || true
  note "MAUI package versions (Microsoft.Maui.*) follow the new SDK line"
  grep -rn "${skip[@]}" --include='*.csproj' -E "Microsoft\.Maui\.[A-Za-z]+\" Version=" . | sed 's/^/    /' || true
  note "CI: .github/workflows/build.yml pins the SDK version used by the release job"
  grep -rn "dotnet-version" .github/workflows/ | sed 's/^/    /' || true
}
