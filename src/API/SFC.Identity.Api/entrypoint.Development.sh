#!/bin/sh

set -e

update-ca-certificates

apt-get update

apt-get install -y curl

dotnet run --project /app/src/API/SFC.Identity.Api/SFC.Identity.Api.csproj --no-launch-profile