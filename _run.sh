#!/bin/sh
set -e

echo '*** executing reverse geocode ***'

dotnet /reverse-geocode/ReverseGeocode.dll AUTO "${DB_CONN}" "${API_KEY}" /results
