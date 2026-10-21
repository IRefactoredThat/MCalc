#!/usr/bin/env bash
# Show the pinned SDK, the locked workload and whether a newer workload is published

set -euo pipefail

# shellcheck source=nix/lib.sh
source "${MCALC_LIB:-$(dirname "$0")/lib.sh}"

require_tools nix curl jq

manifest="$(locked_workload_manifest)"
locked_id="${manifest%% *}"
locked_version="${manifest##* }"

step "Pinned"
note "SDK line:      .NET $(sdk_major) ($(sdk_version))"
note "workload:      $(lock_field workloadVersion)"
note "manifest band: $(lock_field manifestBand)"
note "SDK band:      $(lock_field sdkBand)"

if [ -z "$locked_id" ]; then
  exit 0
fi

step "Published"
latest="$(latest_published_version "$locked_id")"
if [ -z "$latest" ]; then
  note "$locked_id: no published version found"
elif [ "$latest" = "$locked_version" ]; then
  note "$locked_id $latest — up to date"
else
  note "$locked_id $locked_version -> $latest available"
  note "run 'nix run .#update-workload' to pick it up"
fi
