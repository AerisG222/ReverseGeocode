#!/bin/bash
TAG=$(date +%Y%m%d%H%M%S)
IMAGE_ID=$(buildah bud -f Containerfile -t maw-reverse-geocode)
buildah push --creds "${DH_USER}:${DH_PASS}" "${IMAGE_ID}" docker://aerisg222/maw-reverse-geocode:"${TAG}"
