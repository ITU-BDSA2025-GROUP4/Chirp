#!/bin/env bash

PROJECT_ROOT=$(dirname $( cd -- "$( dirname -- "${BASH_SOURCE[0]}" )" &> /dev/null && pwd ))

TEST=test/Chirp.Web.PlayWrightTests
COMPONENT=src/Chirp.Web
DB=playwright_test_db.db
DB_PATH=$PROJECT_ROOT/$COMPONENT/$DB


if [ -e $DB_PATH ]; then
    rm $DB_PATH
fi


CHIRPDBPATH=$DB dotnet run --project $PROJECT_ROOT"/"$COMPONENT &
PID=$!

# File the DB file doesn't exist, assume server hasn't strated yet
while [ ! -f $DB_PATH ]; do
    sleep 1
done

# Just because the server has created the DB, don't mean it's ready
# Wait an extra 3 seconds
sleep 3

cd $PROJECT_ROOT"/"$TEST && dotnet test

kill $PID

