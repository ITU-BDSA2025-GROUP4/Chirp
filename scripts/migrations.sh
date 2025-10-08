#!/usr/bin/env bash

SCRIPT_DIR=$( cd -- "$( dirname -- "${BASH_SOURCE[0]}" )" &> /dev/null && pwd | sed "s/\/scripts$//")

export PATH="$PATH:$HOME/.dotnet/tools/"

dotnet ef migrations add InitialCreate --project=${SCRIPT_DIR}/src/Chirp.Infrastructure --startup-project=${SCRIPT_DIR}/src/Chirp.Web
dotnet ef database update InitialCreate --project=${SCRIPT_DIR}/src/Chirp.Infrastructure --startup-project=${SCRIPT_DIR}/src/Chirp.Web
