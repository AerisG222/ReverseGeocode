#!/bin/bash
set -e

# poor mans cron
while true; do
    sleep 55m

    echo '*** checking if we need to run reverse geocoder ***'

    hour=$(date +"%H")

    if [ "${hour}" = "01" ]; then
        echo '*** executing reverse geocode ***'

        /reverse-geocode/ReverseGeocode AUTO "${DB_CONN}" "${API_KEY}" /results
    fi
done
