# Contributing

Before making a change, read the [development policy](docs/policies/development.md) and open the
[repository policy index](docs/policies/README.md) for the one policy that covers the area you touch.
The [repository guide](docs/repository.md) is a reference: read the section for that area, not the
whole file.

Branches follow the [branching policy](docs/policies/branching.md): `main` holds the released state,
`dev` integrates the next release, and every change arrives through a short-lived `feature/`, `fix/`,
`chore/`, `release/` or `hotfix/` branch and a pull request. The
[pull request policy](docs/policies/pull-requests.md) defines what a pull request must contain.

Commit messages follow the [commit message policy](docs/policies/commits.md).

Tests follow the [testing policy](docs/policies/testing.md): `*.Tests` projects run in CI; a change
that needs a manual check says so in the pull request.

Keep changes focused. A consumer-visible change carries its changelog entry as one fragment in
`changelog.d/`, in the same branch as the change; regular development leaves `## [Unreleased]` in
[CHANGELOG.md](CHANGELOG.md) empty and the release section is composed when a release is prepared.
See the [changelog policy](docs/policies/changelog.md).

Before submitting a pull request, check the items that apply to the change:

- Assess whether tests are needed; add or update tests, following the test approval rule in the
  development policy.
- Build the affected projects and configurations; resolve new compiler and analyzer warnings in
  touched code.
- Review compatibility with saved settings and data files when the change affects them.
- Update the XML documentation and the package [README](README.md) when a public API or a
  consumer-visible behaviour changes.
- Add the changelog fragment when the change is observable by a consumer of the package.
- Run the repository validator after changing repository-level files, documentation or the solution:

```powershell
./scripts/Validate-Repository.ps1
./scripts/Update-Changelog.ps1 -Verify
./scripts/Verify-Release.ps1
```

## Before publishing a release

The [release policy](docs/policies/releases.md) defines the release channels, tags and metadata.

- Verify the release tag and version against the release commit.
- Compose the new `MAJOR.MINOR.PATCH` section from the pending fragments and leave `Unreleased`
  empty, as described in the [changelog policy](docs/policies/changelog.md).
- Confirm CI succeeds before running the publish workflow, and inspect the package on nuget.org when
  it completes.
