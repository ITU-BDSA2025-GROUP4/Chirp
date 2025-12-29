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

![bg right:50% 100%](./images/domain-model.svg)

## Architecture - in the small (WIP)
The project utilizes the onion architecture, and the codebase is organized accordingly. The organization of the code base can be seen in the diagram below:

![bg right:50% 100%](./images/onion.png)\
*The innermost layer is the Domain layer, followed by the Repository layer and Services layer as one combined layer, and finally the UI layer.*

The distinction between the repository and service layers is not strictly enforced, resulting in some overlap of responsibilities. Since the repository and service layers overlap in responsibilities, they can be considered as a single combined layer, resulting in a three-layered diagram as seen above. Nonetheless, the architecture still adheres to the onion architecture in the sense that all dependencies point inwards.

As shown in the diagram above, `Chirp.Core` comprises the domain model, data transfer objects, domain interfaces, and other domain-specific objects. `Chirp.Infrastructure` contains the implementations of the domain interfaces in the form of loosely coupled services and repositories. It also contains the data model and database context. `Chirp.Web` contains the actual web application (the Razor Page application), which includes the web frontend and UI logic that utilizes the services implemented in `Chirp.Infrastructure`.

## Architecture of deployed application
The diagram below shows the deployment architecture of the application. The application follows a client-server architecture. The server component is a monolith deployed on Microsoft Azure. The server processes incoming requests, interacts with its integrated database, and sends back responses. The web browser component on the user's device acts as the client and is capable of exchanging requests and responses with the server over HTTPS, and rendering the received data to the user.

![Deployment Diagram](./images/deployment-diagram.svg)

## User activities

## Sequence of functionality/calls trough _Chirp!_

# Process

## Build, test, release, and deployment

## Team work
**DO NOT FORGET! SHOW A SCREENSHOT OF PROJECT BOARD RIGHT BEFORE HAND-IN AND BRIEFLY DESCRIBE WHICH TASKS ARE STILL UNRESOLVED, I.E. WHICH FEATURES ARE MISSING OR WHICH FUNCTIONALITY IS INCOMPLETE**

Our group follows a simple and structured development workflow from issue creation to feature integration. When a new issue (something that needs to be worked on) is identified, it is documented as an issue on GitHub in the format of a user story. The issue includes a clear task description, suitable acceptance criteria, and relevant labels. The issue is then assigned to one or more group members. The assigned developers then implement the required functionality and test it to verify that it meets the acceptance criteria. Once the work is completed, a pull request is opened against the main branch. Before the pull request is merged, it is automatically reviewed by CodeFactor and at least one team member. If the reviewer(s) approve the changes, the pull request is merged into the main branch. If any issues are identified during code review, the developers revise their implementation and repeat the testing and review process until it is approved. The flow can be seen in the activity diagram below:

![Team Work](./images/team-work.svg)

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
