#!/bin/bash -eu -o pipefail

cd "$(dirname "$0")"

PAKET_EXEC=".paket/paket.exe"

OS=${OS:-"unknown"}
function run() {
   if [[ "$OS" != "Windows_NT" ]]
   then
      FrameworkPathOverride=$(dirname $(dirname $(which mono)))/lib/mono/4.5/ mono "$@"
   else
      "$1" "$@"
   fi
}

run $PAKET_EXEC install
run dotnet build
