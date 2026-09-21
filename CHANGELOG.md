# Changelog

All notable changes to this project are documented in this file.

The format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

## [Unreleased]

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
