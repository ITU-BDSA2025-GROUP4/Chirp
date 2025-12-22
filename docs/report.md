# Design and architecture
## Domain model
## Architecture - in the small
## Architecture of deployed application
## User activities
## Sequence of functionality/calls through Chirp!

# Process
## Build, test, release, and deployment
## Team work
## How to make Chirp! work locally
Ensure that the following dependencies are installed.

`dotnet-runtime 8.0`
`asp-runtime 8.0`
`sqlite3`

## How to run test suite locally

The simplest way to run all unit tests is to simply execute the helper script
`scripts/run_all_tests.sh` like so.

```
./scripts/run_all_tests.sh
```

Alternatively, tests can be run individually by navigating to the appropriate
unit test directory inside the `test` directory and running the .NET test
command.

```
dotnet test
```

# Ethics
## License
The project is licensed under the MIT license.

## LLMs, ChatGPT, CoPilot, and others
During the development of this project, several LLMs were used, namely `ChatGPT`, `GitHub Copilot` and `Google Gemini`. The models were used to support the development of the project, but not as substitutes for independent problem-solving; i.e., they were primarily used for suggesting implementations, clarifying syntax and language-specific features, and proposing alternative approaches to problems. GitHub Copilot was additionally used during code reviews as an automated tool that provided suggestions and caught bugs. Additionally, the LLMs were used to generate CSS for the project.

Overall, the responses from the LLMs were *moderately* helpful. While they rarely produced fully functional code that could be integrated into the project without modification, they were very effective at pointing development in the right direction and offering different perspectives on problems. In this capacity, the LLMs thus functioned as a pair programmer or sparring partner. This helped speed up the project’s development, as having a sparring partner who continuously provided feedback helped with solving problems more efficiently.
