#!/bin/sh

FILEPATH="src/Chirp.CLI/Version.cs"
NAMESPACE="MetaData"
VERSION=${1}

VERSION=$(echo -n "${VERSION}" | grep -Eo "^[^-]+")

if [ -z $VERSION ]; then
    echo "No version number given";
    exit 1;
fi

echo "namespace ${NAMESPACE} { public static class Version { public static string version = \"${VERSION}\";}}" > $FILEPATH






