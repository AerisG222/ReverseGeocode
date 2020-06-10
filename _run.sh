#!/bin/bash
set -e

exec /reverse-geocode/ReverseGeocode AUTO "${DB_CONN}" "${API_KEY}" /results
