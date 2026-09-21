# Revit.Events

[![Revit 2021.1.9, 2023, 2025](https://img.shields.io/badge/Revit-2021.1.9%20%7C%202023%20%7C%202025-green.svg)](https://autodesk.com/revit)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)
[![VolocyNazad](https://img.shields.io/badge/VolocyNazad-blue.svg)](https://github.com/VolocyNazad)

> Service implementation for raising external events in the Revit API.

Revit.Events is a set of services with support for DI containerization, for queuing operations initiated outside the Revit API context and executing them inside the Revit API context.

More about how the mechanism works: [![RevitApi](https://img.shields.io/badge/RevitApi-blue.svg)](https://help.autodesk.com/cloudhelp/2018/ENU/Revit-API/Revit_API_Developers_Guide/Advanced_Topics/External_Events.html)

## Installation

```
dotnet add package VolocyNazad.Revit.Events
```

The package is built per Revit version: the assembly is tied to a specific Revit version — choose the package version matching your Revit API version.

## Features

- `IExternalEvent` — synchronously queues an action on Revit's external event queue, returning an `ExternalEventRequest` immediately.
- `IAsyncExternalEvent` — the async version: returns a `Task` that completes once the action has run inside Revit (including with an error, if the action threw an exception).
- `ExternalEventOptions.AllowDirectInvocation` — if the call is already happening inside the Revit API context, the action runs immediately, without going through Revit's external event queue.

## DI Registration

```csharp
using Revit.Events.DI;

services.AddEvents();
```

Registers `IExternalEvent` and `IAsyncExternalEvent` as singleton services.

## Usage

### Synchronous

```csharp
public class MyCommand
{
    private readonly IExternalEvent _externalEvent;

    public MyCommand(IExternalEvent externalEvent) => _externalEvent = externalEvent;

    public void Run()
    {
        _externalEvent.Raise(uiApplication =>
        {
            // code that requires the Revit API context
        });
    }
}
```

### Asynchronous

```csharp
public class MyCommand
{
    private readonly IAsyncExternalEvent _asyncExternalEvent;

    public MyCommand(IAsyncExternalEvent asyncExternalEvent) => _asyncExternalEvent = asyncExternalEvent;

    public async Task RunAsync()
    {
        await _asyncExternalEvent.Raise(uiApplication =>
        {
            // code that requires the Revit API context
        });
    }
}
```

### AllowDirectInvocation

```csharp
_externalEvent.Raise(uiApplication =>
{
    // ...
}, ExternalEventOptions.AllowDirectInvocation);
```

If the code is already running inside the Revit API context (for example, called from another external event), the action executes immediately — without being queued and waiting for Revit's next "tick".

## Supported Revit Versions

Revit 2021.1.9 and 2023.0.0 use net48; Revit 2025.0.0 uses net8.0-windows.

## Development

The matrix of Revit versions is defined by the build configurations (`Debug_<year>.<...>` / `Release_<year>.<...>`) in `src/Revit.Events/Revit.Events.csproj` and `Toolkit.Revit.Events.slnx`. The package version is computed automatically from git tags via MinVer, tied to the Revit year.

Tests live in `tests/Revit.Events.Tests`. Part of the logic (working with real Revit-typed objects) cannot be tested outside of the Revit process — such tests are marked `[Fact(Skip = "...")]`.

```
dotnet build --configuration "Release_2025.0.0"
dotnet test --configuration "Release_2025.0.0"
```

## Changelog

See [CHANGELOG.md](./CHANGELOG.md).

## License

[MIT](./LICENSE.md)

## Development documentation

- [Development policy](docs/policies/development.md)
- [Repository guide and technology stack](docs/repository.md)

## Contributing
 [CONTRIBUTING.md](CONTRIBUTING.md) before submitting changes.
