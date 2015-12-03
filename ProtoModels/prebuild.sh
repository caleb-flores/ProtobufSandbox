#!/usr/bin/env bash
PATH=/usr/local/bin:$PATH

echo "Starting....."
set -eu
set -o pipefail

cd `dirname $0`

FSIARGS=""
OS=${OS:-"unknown"}
if [[ "$OS" != "Windows_NT" ]]
then
  FSIARGS="--fsiargs -d:MONO"
fi

function run() {
  if [[ "$OS" != "Windows_NT" ]]
  then
    mono "$@"
  else
    "$@"
  fi
}



echo "running FAKE task"
run ../packages/FAKE/tools/FAKE.exe "$@" $FSIARGS prebuild.fsx