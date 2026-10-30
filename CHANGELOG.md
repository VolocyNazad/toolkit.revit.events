# Changelog

All notable changes to this project are documented in this file.

The format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

## [Unreleased]

### Added

- Add a testing policy: test kinds, Revit version coverage, naming, isolation and the manual smoke check.
- Add a benchmark policy: when performance is measured, how, and how results are reported.

- Branching policy: `main` holds the released state, `dev` integrates the next release, and changes arrive through feature, fix, release and hotfix branches.

### Changed

- Move the changelog policy to `docs/policies/changelog.md` and require it in the repository validator.

- Move the commit message policy to `docs/policies/commits.md` and require it in the repository validator.

- Document the commit message policy: the Conventional Commits type set, breaking-change markers and subject rules.

- Moved package versions to central package management (`Directory.Packages.props` and `tests/Directory.Packages.props`); Revit API package versions follow the selected build configuration.

## [0.0.10] - 2026-10-15

### Added

- Support Revit 2021-2027: 2021.1.100, 2022.1.80, 2023.1.90, 2024.3.60, 2025.4.60, 2026.4.10, 2027.3.0 (SDK 1.1.0; 2021-2024 net48, 2025-2026 net8.0-windows, 2027 net10.0-windows).

### Changed

- Reference the Revit API through `Nice3point.Revit.Api.RevitAPI` / `Nice3point.Revit.Api.RevitAPIUI` (floating `$(MajorRevitVersion).*`) instead of `Revit_All_Main_Versions_API_x64`, so each major resolves to its latest stable API patch.

## [0.0.9] - 2026-09-21

### Added

- Validate required repository files, navigation links, and Solution Items in CI.

### Fixed

- Allow scenario-based names on test classes while retaining naming analysis elsewhere.

- Correct XML documentation references on the external-event interfaces.

- Fail with explicit reflection errors when required internal Revit API members cannot be found.

- Correct the repository guide to reflect that central package management is not used.

- Include the build configuration in CI artifact names to prevent collisions between Revit matrix jobs.

### Changed

- Standardize GitHub Actions workflow filenames and display names by responsibility.

- Limit the declared Revit support matrix to 2021.1.9, 2023.0.0, and 2025.0.0.

- Configure xUnit v3 test execution through Microsoft.Testing.Platform and fail test runs when no tests are discovered.

- Establish a shared EditorConfig baseline and use the repository-policy validator as the single structural CI check.

- Complete solution items for repository documents, configuration, workflows and maintenance scripts; document the shared layout.

- Standardize local and CI SDK selection on stable .NET 10.0 through global.json, restrict roll-forward to that major/minor line, and configure setup-dotnet to read the file.

- Show documentation in Visual Studio Solution Explorer under a `docs` solution folder with matching subfolders.

- Move development policies and repository guidance from `AGENTS.md` to `docs/policies/development.md` and `docs/repository.md`; keep required reading links in `AGENTS.md` and add README navigation.
