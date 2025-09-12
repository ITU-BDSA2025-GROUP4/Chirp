#!/bin/sh

FILEPATH="src/Chirp.CLI/Version.cs"
NAMESPACE="MetaData"
VERSION_RAW=${1}

VERSION=$(echo -n "${VERSION_RAW}" | grep -Eo "^[^-]+")

if [ -z $VERSION ]; then
    echo "No version number given: Raw version: ${VERSION_RAW}, trimmed version: ${VERSION}";
    exit 1;
fi

echo "namespace ${NAMESPACE} { public static class Version { public static string version = \"${VERSION}\";}}" > $FILEPATH






