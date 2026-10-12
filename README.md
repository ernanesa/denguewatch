# DengueWatch — DIO .NET and MongoDB Challenge

[English](README.md) | [Português (BR)](README.pt-BR.md)

An **educational** portfolio project using synthetic data, inspired by DIO's lab
"Building a .NET API integrated with MongoDB". The goal is to practice Controllers, dependency
injection, MongoDB, Docker Compose, JWT authentication, and testing — not to serve real users.

## Challenge context

The portfolio challenge asks the learner to create their own repository, replicate or improve the
demonstrated project, and include the links and files needed to understand it. Forking the reference
repository is optional.

- Lab and lesson links: [projetoNetMongo.md](projetoNetMongo.md)
- Reference implementation: [gabrielfaraday/dotnet-mongo](https://github.com/gabrielfaraday/dotnet-mongo)

The reference repository demonstrates a small MongoDB-backed API. DengueWatch is its own adaptation,
and its API contract is **not** assumed to match the reference `/infectado` endpoint.

## Project decisions

- .NET 10 (`net10.0`) with ASP.NET Core Controllers.
- `WebApplication.CreateBuilder` (default builder).
- MongoDB via `MongoDB.Driver`, running locally through Docker Compose.
- OpenAPI and Scalar enabled only in `Development`.
- Educational authentication: login and password issuing a JWT. This is a learning simplification,
  not a production-ready identity solution.
- Synthetic data only.

These are **DengueWatch** decisions, not official requirements stated by the challenge text.

## Current status

The repository currently holds the application **scaffold**: MongoDB configuration and dependency
injection, Docker/Compose, Controllers, sample DTOs, OpenAPI/Scalar, and a basic `/health` endpoint.

Not implemented yet: case CRUD, MongoDB read/write operations, login/JWT, and automated tests. The
last review compiled the API successfully, but integration and tests remain unverified.

The detailed plan with milestones and acceptance criteria is in [plan/README.md](plan/README.md).

## Run locally

```bash
docker compose up --build
```

The API is available at `http://localhost:5000`. In `Development`, Scalar is at
`http://localhost:5000/scalar/v1`.

## Data and license

Educational project. Do not use real patient or health data.
