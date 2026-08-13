# Revit.Events

[![Revit 2011-2027](https://img.shields.io/badge/Revit-2011–2027-green.svg)](https://autodesk.com/revit)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)
[![VolocyNazad](https://img.shields.io/badge/VolocyNazad-blue.svg)](https://github.com/VolocyNazad)

> Реализация сервисов для вызова внешних событий в Revit API.

Revit.Events — это набор сервисов с поддержкой DI-контейнеризации для вызова выполнения операций вне контекста Revit API и их выполнения внутри контекста Revit API.

Подробнее о механизме работы: [![RevitApi](https://img.shields.io/badge/RevitApi-blue.svg)](https://help.autodesk.com/cloudhelp/2018/ENU/Revit-API/Revit_API_Developers_Guide/Advanced_Topics/External_Events.html)

## Установка

```
dotnet add package VolocyNazad.Revit.Events
```

Пакет паковый: сборка привязана к конкретной версии Revit — выбирайте версию пакета под свою версию Revit API.

## Возможности

- `IExternalEvent` — синхронная постановка действия в очередь внешних событий Revit, немедленно возвращает `ExternalEventRequest`.
- `IAsyncExternalEvent` — асинхронная версия: возвращает `Task`, который завершается после выполнения действия внутри Revit (в т.ч. с ошибкой, если действие выбросило исключение).
- `ExternalEventOptions.AllowDirectInvocation` — если вызов уже происходит внутри контекста Revit API, действие выполняется немедленно, без похода через очередь внешних событий Revit.

## Регистрация в DI

```csharp
using Revit.Events.DI;

services.AddEvents();
```

Регистрирует `IExternalEvent` и `IAsyncExternalEvent` как singleton-сервисы.

## Использование

### Синхронно

```csharp
public class MyCommand
{
    private readonly IExternalEvent _externalEvent;

    public MyCommand(IExternalEvent externalEvent) => _externalEvent = externalEvent;

    public void Run()
    {
        _externalEvent.Raise(uiApplication =>
        {
            // код, требующий контекста Revit API
        });
    }
}
```

### Асинхронно

```csharp
public class MyCommand
{
    private readonly IAsyncExternalEvent _asyncExternalEvent;

    public MyCommand(IAsyncExternalEvent asyncExternalEvent) => _asyncExternalEvent = asyncExternalEvent;

    public async Task RunAsync()
    {
        await _asyncExternalEvent.Raise(uiApplication =>
        {
            // код, требующий контекста Revit API
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

Если код уже выполняется внутри контекста Revit API (например, вызван из другого внешнего события), действие выполнится сразу — без постановки в очередь и ожидания следующего "тика" Revit.

## Поддерживаемые версии Revit

Revit 2011–2027 (net48 для версий до 2025, net8.0-windows для 2025+).

## Разработка

Матрица версий Revit задаётся конфигурациями сборки (`Debug_<год>.<...>` / `Release_<год>.<...>`) в `src/Revit.Events/Revit.Events.csproj` и `Toolkit.Revit.Events.slnx`. Версия пакета вычисляется автоматически из git-тегов через MinVer, привязываясь к году Revit.

Тесты — в `tests/Revit.Events.Tests`. Часть логики (обращение к реальным Revit-типизированным объектам) не может быть протестирована вне процесса Revit — такие тесты помечены `[Fact(Skip = "...")]`.

```
dotnet build --configuration "Release_2025.0.0"
dotnet test --configuration "Release_2025.0.0"
```

## Changelog

См. [CHANGELOG.md](./CHANGELOG.md).

## Лицензия

[MIT](./LICENSE.md)
