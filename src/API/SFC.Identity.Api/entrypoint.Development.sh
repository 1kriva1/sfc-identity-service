#!/bin/sh

./src/API/SFC.Identity.Api/entrypoint.Common.sh
dotnet run --project /app/src/API/SFC.Identity.Api/SFC.Identity.Api.csproj --no-launch-profile