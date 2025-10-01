#!/usr/bin/env bash
echo export PATH="$PATH:$HOME/.dotnet/tools/"

# Should probably use descriptive names for the migrations, cba tho
# Also maybe we can do this in the c# file rather that this bash script
dotnet ef migrations add InitialCreate
dotnet ef database update
