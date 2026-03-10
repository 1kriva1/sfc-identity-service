#!/bin/sh

./entrypoint.Common.sh

dotnet watch run --project $1 --no-launch-profile