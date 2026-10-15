# Repository guide

Paths in this document are relative to the repository root.
Read and follow the [development policy](policies/development.md) alongside this guide.

## About

Source for `VolocyNazad.Revit.Events`: wraps Revit API events (document
changed, view activated, idling, etc.) behind a friendlier subscription
API. Consumed by `toolkit.revit.mediatR` in this same family.

## Repository structure

```
.
├── src/
│   └── Revit.Events/            the library
└── tests/
    └── Revit.Events.Tests/
```

## Tech stack

- .NET, built against `Nice3point.Revit.Api.RevitAPI` /
  `Nice3point.Revit.Api.RevitAPIUI` (floating `$(MajorRevitVersion).*`, so each
  major resolves to its latest stable API patch)
- AutoConstructor (source-generated DI constructors)
- Microsoft.Extensions.DependencyInjection.Abstractions
- MinVer (git-tag-based versioning), PolySharp (polyfills)
- Tests: **xunit.v3** (matches `toolkit.revit.async` in this same repo family) + coverlet.collector +
  Microsoft.Extensions.DependencyInjection
- Central package management is not used; package versions are set per
  `<PackageReference>`.

## Documentation layout

- `AGENTS.md` links to the required repository guidance.
- `docs/policies/development.md` contains the development policy.
- `docs/repository.md` describes the project, repository structure, and technology stack.

The root solution exposes the documentation files under a `docs` solution folder in Visual Studio, preserving their subfolder structure. When adding documentation files, also add them as solution items; solution folders do not automatically include new files.

CI artifact names include the build configuration so each Revit matrix job uploads a distinct artifact. NuGet publishing builds its own packages and does not download these CI artifacts.

The root `global.json` selects stable .NET SDK 10.0 (minimum `10.0.103`, `rollForward: latestFeature`). CI and publishing install the SDK from this file. Additional SDK installations may provide older test runtimes. See the [SDK selection policy](policies/development.md#net-sdk-selection).

## Solution items

The root solution exposes repository-level documents and configuration under `solutionItems`, GitHub files and maintenance scripts in matching subfolders, and documentation under `docs/`. The list is explicit, not a filesystem glob; keep links up to date when files change. See the [solution items policy](policies/development.md#solution-items).
## Repository validation

`scripts/Validate-Repository.ps1` enforces the required repository documents,
their navigation links, and complete, valid Solution Items. The
`.github/workflows/repository-policy.yml` workflow runs it for pushes and pull
requests. See the [development policy](policies/development.md#repository-validation).

## Formatting

The root `.editorconfig` defines the portable formatting baseline. Existing repositories may add stricter C# or analyzer-specific settings. See the [development policy](policies/development.md#formatting-baseline).

## Versioning and release tags

The library uses MinVer 8 with stable tags in the `0.0.PATCH` format and no `v` prefix. A tagged commit is built separately for Revit 2021.1.100, 2022.1.80, 2023.1.90, 2024.3.60, 2025.4.60, 2026.4.10, and 2027.3.0; tag `0.0.10`, for example, produces package versions `2021.0.10` through `2027.0.10`. Manual publishing accepts exactly one matching tag at HEAD and verifies the package ID and Revit-specific version before pushing to NuGet.

The tagged commit must already contain the release section in `CHANGELOG.md` with an empty `Unreleased` section above it; the publish workflow stops otherwise. See the [release changelog policy](policies/development.md#release-changelog).
