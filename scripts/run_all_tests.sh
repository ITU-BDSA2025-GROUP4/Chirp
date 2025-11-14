#!/bin/sh

PROJECT_ROOT=$(dirname $( cd -- "$( dirname -- "${BASH_SOURCE[0]}" )" &> /dev/null && pwd ))

set -xe pipefail

# This is used for running every single unit test on Unix system in the terminal.
# Run this from the project root, like so: ./scripts/run_all_tests.sh

# PlayWright is excluded from here because it needs some extra setup to run
for test in $(find $PROJECT_ROOT/test/ -maxdepth 1 -type d | tail +2 | grep -v "PlayWright"); do 
    (cd $test && dotnet test);
done

$PROJECT_ROOT/scripts/playwright_test.sh

