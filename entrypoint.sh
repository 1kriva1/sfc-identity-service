#!/bin/sh
set -e

update-ca-certificates
apt-get update
apt-get install -y curl

dotnet watch run --project $1 --no-launch-profile