#!/bin/sh
set -e

update-ca-certificates
apt-get update
apt-get install -y curl