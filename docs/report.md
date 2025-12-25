---
title: _Chirp!_ Project Report
subtitle: ITU BDSA 2025 Group 4
author:
- "Adam Yaser Sweilem  <adsw@itu.dk>"
- "Ask Filip Christensen <asfc@itu.dk>"
- "August Leander Hedman <aulh@itu.dk>"
- "Morgan Møller Torp <moto@itu.dk>"
- "Vitus Ammitzbøll Brodersen <vitb@itu.dk>"
numbersections: true
---

# Design and Architecture of _Chirp!_

## Domain model

The domain model consists of 4 concrete classes and an abstract class which
stems from the .NET Identity library.

Users on the *Chirp!* platform are represented with the **Author** class. A
follow relationship between two users is represented using the **Follow**
class. The **Cheep** class represents messages an **Author** can make. Finally,
**Reply** is the class used for representing replies to messages.

![bg right:50% 100%](img/chirp_domain_model.jpg)

## Architecture - in the small


## Architecture of deployed application
The diagram below shows the deployment architecture of the application. The application follows a client-server architecture. The server component is a monolith deployed on Microsoft Azure. The server processes incoming requests, interacts with its integrated database, and sends back responses. The web browser component on the user's device acts as the client and is capable of exchanging requests and responses with the server over HTTPS, and rendering the received data to the user.

![Deployment Diagram](./images/deployment-diagram.svg)

## User activities

## Sequence of functionality/calls trough _Chirp!_

# Process

## Build, test, release, and deployment

## Team work

## How to make _Chirp!_ work locally
Ensure that the following dependencies are installed:
- `dotnet-runtime 8.0`
- `asp-runtime 8.0`
- `sqlite3`

Ensure that the database migrations are up to date by using the migration
helper script. For the migration_name, use anything that doesn't appear in
`src/Chirp.Infrastructure/Migrations`.
```
./scripts/migration.sh <MIGRATION_NAME>
```

Compile and run the razor app.
```
dotnet run --project src/Chirp.Web
```

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
During the development of this project, several LLMs were used, namely `ChatGPT`, `GitHub Copilot` and `Google Gemini`. The models were used to support the development of the project, but not as substitutes for our own problem-solving, i.e., they were primarily used for suggesting implementations, clarifying syntax and language-specific features, and proposing alternative approaches to problems. GitHub Copilot was additionally used during code reviews as an automated tool that provided suggestions and caught bugs. Additionally, the LLMs were used to generate CSS for the project.

Overall, the responses from the LLMs were *moderately* helpful. While they rarely produced fully functional code that could be integrated into the project without modification, they were very effective at pointing development in the right direction and offering different perspectives on problems. In this capacity, the LLMs thus functioned as a pair programmer or sparring partner. This helped speed up the project’s development, as having a sparring partner who continuously provided feedback helped with solving problems more efficiently.
