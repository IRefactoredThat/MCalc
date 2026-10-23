#!/usr/bin/env bash
set -euo pipefail

REPO_ROOT="$(cd "$(dirname "$0")/.." && pwd)"
DEVICE="${1:-e12092a8}"
CONFIG="${2:-Release}"

WORKLOAD_SDK=""
if [ -n "${DOTNET_ROOT:-}" ] && [ -d "$DOTNET_ROOT/packs/Microsoft.Android.Sdk.Linux" ]; then
  WORKLOAD_SDK="$DOTNET_ROOT"
fi
if [ -z "$WORKLOAD_SDK" ]; then
  for d in /nix/store/*-dotnet-sdk-with-android-workload-*/; do
    [ -d "$d" ] || continue
    d="${d%/}"
    [[ "$d" == *.drv || "$d" == *.lock ]] && continue
    [ -d "$d/packs/Microsoft.Android.Sdk.Linux" ] && [ -d "$d/sdk-manifests" ] && { WORKLOAD_SDK="$d"; break; }
  done
fi

REAL_SDK=""
for cand in "/nix/store/b8lqbpbgy4lyp46dwxks5x5i8mrvc806-dotnet-sdk-10.0.401/share/dotnet" "/nix/store/hzv8gh10apyd26whwx8sg4lsaxjgwssd-dotnet-sdk-10.0.401/share/dotnet"; do
  [ -x "$cand/dotnet" ] && { REAL_SDK="$cand"; break; }
done
[ -z "$REAL_SDK" ] && for d in /nix/store/*-dotnet-sdk-10.0.401*/share/dotnet; do
  [ -d "$d" ] && [ -x "$d/dotnet" ] && { REAL_SDK="$d"; break; }
done

[ -z "$WORKLOAD_SDK" ] || [ -z "$REAL_SDK" ] && { echo "SDKs not found"; exit 1; }

rm -rf "$HOME/.dotnet/packs/Microsoft.Android.Sdk.Linux" "$HOME/.dotnet/sdk-manifests/10.0.100/microsoft.net.sdk.android" 2>/dev/null || true
mkdir -p "$HOME/.dotnet/packs" "$HOME/.dotnet/sdk-manifests/10.0.100"
ln -sf "$WORKLOAD_SDK/packs/Microsoft.Android.Sdk.Linux" "$HOME/.dotnet/packs/"
ln -sf "$WORKLOAD_SDK/sdk-manifests/10.0.100/microsoft.net.sdk.android" "$HOME/.dotnet/sdk-manifests/10.0.100/"

export DOTNET_ROOT="$REAL_SDK"
export DOTNET_HOST_PATH="$REAL_SDK/dotnet"
export PATH="$REAL_SDK:$PATH"
export DOTNET_MULTILEVEL_LOOKUP=0
export MSBUILDDISABLENODEREUSE=1
export DOTNET_NOLOGO=1
export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
export DOTNETSDK_WORKLOAD_MANIFEST_ROOTS="$HOME/.dotnet/sdk-manifests"
export DOTNETSDK_WORKLOAD_PACK_ROOTS="$HOME/.dotnet/packs"
export NUGET_PACKAGES="$REPO_ROOT/.nuget/packages"
export MSBUILDTASKHOST=InProcess
export EnableNETSdkTaskHost=false

cd "$REPO_ROOT"
rm -rf CalculatorApp/obj CalculatorApp/bin

exec dotnet run \
  --project CalculatorApp/CalculatorApp.csproj \
  -c "$CONFIG" \
  --device "$DEVICE" \
  -p:UseSharedCompilation=false \
  -p:RunAOTCompilation=false
