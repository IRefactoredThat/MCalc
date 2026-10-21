#!/usr/bin/env bash
# Move the toolchain to another .NET SDK line, refresh the workload and rebuild.

set -euo pipefail

# shellcheck source=nix/lib.sh
source "${MCALC_LIB:-$(dirname "$0")/lib.sh}"

require_tools nix curl jq

target="${1:-}"
[ -n "$target" ] || die "upgrade-sdk needs a target SDK line, e.g. 'nix run .#upgrade-sdk <version>'"
case "$target" in
'' | *[!0-9]*) die "the target SDK line must be a number, got '$target'" ;;
esac

current="$(sdk_major)"
[ -n "$current" ] || die "could not read the SDK line from $PIN_FILE"

target_version="$(sdk_version_for "$target")" || die "nixpkgs does not ship a .NET $target SDK"

snapshot_state
trap restore_state EXIT

step "Moving the toolchain from .NET $current to .NET $target"
note "nixpkgs ships .NET $target as $target_version"
set_sdk_major "$target"
note "pinned .NET $target"

refresh_workload_lock
build_toolchain
print_summary
if [ "$target" != "$current" ]; then
  print_upgrade_followups "$target" "$current"
fi
