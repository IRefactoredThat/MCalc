#!/usr/bin/env bash
# Update the nixpkgs pin to the newest .NET SDK patch of the pinned line,
# refresh the workload and rebuild the toolchain.

set -euo pipefail

# shellcheck source=nix/lib.sh
source "${MCALC_LIB:-$(dirname "$0")/lib.sh}"

require_tools nix curl jq

snapshot_state
trap restore_state EXIT

before_sdk="$(sdk_version)"

step "Updating the nixpkgs pin"
nix flake update nixpkgs
note "nixpkgs updated"

after_sdk="$(sdk_version)"
if [ "$before_sdk" = "$after_sdk" ]; then
  note "nixpkgs still ships .NET $(sdk_major) $after_sdk"
else
  note "SDK $before_sdk -> $after_sdk"
fi

refresh_workload_lock
build_toolchain
print_summary
