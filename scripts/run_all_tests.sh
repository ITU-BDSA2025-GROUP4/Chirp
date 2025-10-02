#!/bin/sh

# This is used for running every single unit test on Unix system in the terminal.
# Run this from the project root, like so: ./scripts/run_all_tests.sh

for test in $(find test/ -maxdepth 1 -type d | grep -v "^test/$"); do (cd $test && dotnet test); done
