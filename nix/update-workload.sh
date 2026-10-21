#!/usr/bin/env bash
# Refresh the pinned .NET Android workload and rebuild the toolchain.

set -euo pipefail

# shellcheck source=nix/lib.sh
source "${MCALC_LIB:-$(dirname "$0")/lib.sh}"

require_tools nix curl jq

snapshot_state
trap restore_state EXIT

before="$(lock_field workloadVersion)"

refresh_workload_lock
build_toolchain

after="$(lock_field workloadVersion)"
if [ "$before" = "$after" ]; then
  step "The pinned workload $after is already the newest published one"
else
  step "Workload $before -> $after"
fi
print_summary
