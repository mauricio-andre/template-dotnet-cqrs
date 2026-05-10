# Repository Guidelines

## Project Structure & Module Organization

This repository packages a .NET CQRS template. The installable template lives in `cqrs-project/`, with the solution at `cqrs-project/CqrsProject.sln`.

- `cqrs-project/src/Apps/`: executable hosts, including REST, gRPC, and database migrator apps.
- `cqrs-project/src/Core/`: domain modules, use cases, entities, shared responses, and common types.
- `cqrs-project/src/Providers/`: infrastructure integrations such as Postgres, Auth0, OpenTelemetry, Swagger, Scalar, caching, localization, and console formatting.
- `cqrs-project/tests/`: xUnit test projects grouped by app and shared test utilities.
- `cqrs-project/docs/` and provider-level `docs/`: project documentation.

## Build, Test, and Development Commands

Run commands from the repository root unless noted.

```bash
dotnet new install ./
dotnet new cqrs-project -n MyProject
dotnet restore cqrs-project/CqrsProject.sln
dotnet build cqrs-project/CqrsProject.sln
dotnet test cqrs-project/CqrsProject.sln
dotnet run --project cqrs-project/src/Apps/CqrsProject.App.RestServer
dotnet run --project cqrs-project/src/Apps/CqrsProject.App.GrpcServer
dotnet run --project cqrs-project/src/Apps/CqrsProject.App.DbMigrator
```

Use the migration commands in `cqrs-project/README.md` when changing EF Core models. Some tests use Testcontainers PostgreSQL, so Docker must be available for the full test suite.

## Coding Style & Naming Conventions

Follow the root `.editorconfig`: 4-space indentation, final newlines, trimmed trailing whitespace, and 2-space JSON indentation. C# braces go on new lines. Prefer explicit types over `var`. Private and internal fields use `_camelCase`; private/internal static fields use `s_camelCase`; constants use `PascalCase`.

Keep domain behavior organized by use case. Under each domain folder, place command/query, handler, validator, event, and use-case-specific response files together in `UseCases/<UseCaseName>/`, for example `Tenants/UseCases/CreateTenant/`. Keep responses that are shared by more than one use case in the domain `Responses/` folder for now.

## Testing Guidelines

Tests use xUnit with `Microsoft.NET.Test.Sdk`; REST tests also use `Microsoft.AspNetCore.Mvc.Testing`, `NSubstitute`, and shared helpers from `tests/Commons`. Name test classes with the `Test` suffix, matching existing examples like `TenantMiddlewareTest` and `DbMigratorTest`. Add or update tests for handler, middleware, endpoint, and migration behavior changes.
Every test must declare a clear `DisplayName` that describes the expected behavior, especially for `Fact` and `Theory` cases.
In `tests/Core`, mirror the `Core` layout by domain. Keep use case tests in `UseCases/<UseCaseName>/` and rule tests in `Rules/` under the same domain folder.
For rule tests, always cover both the success path and the failure path. For use cases that can throw in more than one branch, add a test for each exception path and keep the scenario focused on the behavior that fails.
When a rule handles more than one event and the logic is the same, prefer a `Theory` with one row per event so each supported notification stays visible in the test output.
Keep test data helpers inside the domain folder they serve, such as `Identity/IdentityTestData.cs` or `Tenants/TenantsTestData.cs`, instead of sharing a single global fixture across unrelated domains.

## Commit & Pull Request Guidelines

Recent commits are short and direct, often Portuguese, with occasional Conventional Commit prefixes such as `fix:` and `feat:`. Prefer `feat: ...`, `fix: ...`, or a concise imperative summary. Pull requests should describe the behavior change, list test evidence (`dotnet test cqrs-project/CqrsProject.sln`), mention migration or configuration changes, and link related issues.

## Security & Configuration Tips

Do not commit secrets in `appsettings.json` or launch profiles. Keep Auth0, connection strings, and telemetry endpoints in user secrets, environment variables, or deployment configuration.
