# Contributing

Thank you for your interest in contributing to Majo.Logging.

Majo.Logging is a small logging library built on Serilog. Contributions should keep the project focused and prefer simple, concrete changes over unnecessary abstraction.

## Development Requirements

The library targets:

- .NET 8
- .NET 10

The test project currently targets .NET 10.

## Build

Restore and build the solution with:

```bash
dotnet restore Majo.Logging.slnx
dotnet build Majo.Logging.slnx
```

For a release build:

```bash
dotnet build Majo.Logging.slnx -c Release
```

## Testing

`Majo.Logging.Test` uses xUnit.

Run the automated tests with:

```bash
dotnet test Majo.Logging.Test/Majo.Logging.Test.csproj
```

The logger is process-wide and static, so the test suite runs without parallel test execution.

When changing observable logging behavior, add or update tests when the behavior is meaningful enough to protect against regression. Do not add tests only to increase test count or coverage.

## Local NuGet Package Validation

`Majo.Logging.Test` can consume `Majo.Logging` in two ways.

By default:

```xml
<UsePackageReference>false</UsePackageReference>
```

the test project uses a `ProjectReference`. This is the normal mode for day-to-day development.

When `UsePackageReference` is `true`, the test project instead consumes the locally packed NuGet package.

```text
ProjectReference
    → normal development

PackageReference
    → local NuGet package validation
```

This mode is intended for validating package consumption when packaging or project structure changes.

## Public API

Keep the public API small and focused.

The library should continue to provide a simple logging layer without unnecessarily exposing Serilog-specific implementation details to callers.

Prefer extending the existing model only when there is a concrete use case.

## Pull Requests

Please keep pull requests focused on one logical change.

Before submitting a pull request:

- make sure the solution builds successfully;
- run the automated tests;
- update user-facing documentation when public API or observable behavior changes;
- explain the motivation and main changes in the pull request description.

## Code Style

Follow the existing C# style and project structure where practical.

Prefer small, focused changes over unrelated cleanup or broad refactoring in the same pull request.
