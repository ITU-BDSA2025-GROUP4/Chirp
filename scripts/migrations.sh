#!/usr/bin/env bash

SCRIPT_DIR=$( cd -- "$( dirname -- "${BASH_SOURCE[0]}" )" &> /dev/null && pwd | sed "s/\/scripts$//")

MIGRATION_NAME=$1

if [ -z $MIGRATION_NAME ]; then
    echo "Must provide migration name as an argument, i.e. './scripts/migrations.sh ExampleName'"
    exit 0
fi

export PATH="$PATH:$HOME/.dotnet/tools/"

dotnet ef migrations add ${MIGRATION_NAME} --project=${SCRIPT_DIR}/src/Chirp.Infrastructure --startup-project=${SCRIPT_DIR}/src/Chirp.Web
dotnet ef database update ${MIGRATION_NAME} --project=${SCRIPT_DIR}/src/Chirp.Infrastructure --startup-project=${SCRIPT_DIR}/src/Chirp.Web
