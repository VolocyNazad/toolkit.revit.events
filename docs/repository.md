# Repository guide

Paths in this document are relative to the repository root.
Read and follow the [development policy](policies/development.md) alongside this guide.

## About

Source for `VolocyNazad.Revit.Events`: services that queue work started outside the Revit API
context and execute it inside that context through Revit's external-event mechanism. The library
exposes `IExternalEvent` for fire-and-forget raising and `IAsyncExternalEvent` for awaiting the
result, plus dependency-injection registration.

## Repository structure

```
.
├── src/
│   └── Revit.Events/               the library
└── tests/
    └── Revit.Events.Tests/         headless tests
```

- `Toolkit.Revit.Events.slnx` - solution with the Revit configuration matrix.
- `Directory.Build.props` - shared compiler settings, `AnalysisMode` and the `x64` platform.
- `Directory.Packages.props` - central package versions, including the Revit API version selected by
  the build configuration.
- `tests/Directory.Packages.props` - test-only package versions, importing the root file.
- `global.json` - .NET SDK selection and the Microsoft.Testing.Platform runner.
- `.github/workflows/` - CI, repository policy and publishing workflows.

## Technology stack

- `Microsoft.NET.Sdk` through the `VolocyNazad.Revit.Sdk` MSBuild SDK; the target framework follows
  the Revit version: `net48` for 2021-2024, `net8.0-windows` for 2025-2026 and `net10.0-windows` for
  2027. The platform is `x64` and the library enables WPF.
- Built against `Nice3point.Revit.Api.RevitAPI` / `Nice3point.Revit.Api.RevitAPIUI`; the package
  version follows the selected build configuration (for example `Release_2026.4.10` uses `2026.4.10`).
- C# 14 through the SDK, nullable reference types and implicit usings; AutoConstructor generates the
  dependency-injection constructors, and PolySharp supplies the polyfills.
- `Microsoft.Extensions.DependencyInjection.Abstractions` for the registration API.
- MinVer derives the package version from stable `0.0.PATCH` Git tags; the Revit year becomes the
  package major version.
- Tests use xunit.v3 through Microsoft.Testing.Platform and run outside Revit; tests that need the
  Revit process are marked `[Fact(Skip = "...")]`.
- Package versions are managed centrally in `Directory.Packages.props`; test-only packages live in
  `tests/Directory.Packages.props`, which imports the root file.

## Documentation layout

- `AGENTS.md` links to the required repository guidance and routes a change to the document that owns it.
- `docs/policies/` contains the repository policies - the rules a change must follow. The
  [policy index](policies/README.md) says what each policy covers.
- `docs/repository.md` describes the project, repository structure, and technology stack.
- `changelog.d/` holds the pending changelog fragments: a consumer-visible change adds one, and the
  release preparation composes them into `CHANGELOG.md`; the rules are in the
  [changelog policy](policies/changelog.md). Git does not track empty directories, so the folder
  exists only while a fragment is pending.
- `scripts/` holds the repository validator, the changelog tool and the release verification.

The root solution exposes the documentation files under a `docs` solution folder in Visual Studio, preserving their subfolder structure. When adding documentation files, also add them as solution items; solution folders do not automatically include new files.

CI artifact names include the build configuration so each Revit matrix job uploads a distinct artifact. NuGet publishing builds its own packages and does not download these CI artifacts.

The root `global.json` selects stable .NET SDK 10.0 (minimum `10.0.103`, `rollForward: latestFeature`). CI and publishing install the SDK from this file. Additional SDK installations may provide older test runtimes. See the [SDK selection policy](policies/build.md#sdk-selection).

## Solution items

The root solution exposes repository-level documents and configuration under `solutionItems`, GitHub files and maintenance scripts in matching subfolders, and documentation under `docs/`. The list is explicit, not a filesystem glob; keep links up to date when files change. See the [solution items policy](policies/repository-maintenance.md#solution-items).

## Repository validation

`scripts/Validate-Repository.ps1` enforces the required repository documents,
their navigation links, the complete and valid Solution Items, and the workflow action pins. The
`.github/workflows/repository-policy.yml` workflow runs it, the pending fragment check
(`Update-Changelog.ps1 -Verify`) and the release structure check (`Verify-Release.ps1`) for pushes
and pull requests. See the [repository maintenance policy](policies/repository-maintenance.md#repository-validation).

## Formatting

The root `.editorconfig` defines the portable formatting baseline. Existing repositories may add stricter C# or analyzer-specific settings. See the [repository maintenance policy](policies/repository-maintenance.md#formatting-baseline).

## Continuous integration

`build-and-test` builds every Revit configuration in the matrix with `-p:Platform=x64`, warnings as
errors and documentation warnings promoted to errors, runs the headless test executable and uploads
one artifact per configuration. `repository-policy` runs the validator, the pending fragment check
and the release structure check. `publish-nuget` is manual and refuses a commit without a single
stable tag and a matching changelog section.

## Testing

The automated suite is in `tests/Revit.Events.Tests` and covers the option flags, the lambda handler
and the dependency-injection registration without a running Revit. A test that needs Revit-typed
objects is skipped with the reason in the attribute, and the [testing policy](policies/testing.md)
defines what a manual check must cover.

## Versioning and release tags

The library uses MinVer 8 with stable tags in the `0.0.PATCH` format and no `v` prefix. A tagged
commit is built separately for Revit 2021.1.100, 2022.1.80, 2023.1.90, 2024.3.60, 2025.4.60,
2026.4.10, and 2027.3.0; tag `0.0.10`, for example, produces package versions `2021.0.10`,
`2022.0.10`, `2023.0.10`, `2024.0.10`, `2025.0.10`, `2026.0.10`, and `2027.0.10`.
`scripts/Verify-Release.ps1 -FromHeadTag` accepts exactly one matching tag at `HEAD`, and the publish
workflow verifies the package ID and the Revit-specific version before pushing to NuGet.

The tagged commit must already contain the release section in `CHANGELOG.md` with an empty
`Unreleased` section above it and no pending fragment; the publish workflow stops otherwise. See the
[release policy](policies/releases.md) and the [changelog policy](policies/changelog.md#release).
