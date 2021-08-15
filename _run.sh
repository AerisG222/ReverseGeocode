#!/bin/bash
set -e

echo '*** executing reverse geocode ***'

./ReverseGeocode AUTO "${DB_CONN}" "${API_KEY}" /results
