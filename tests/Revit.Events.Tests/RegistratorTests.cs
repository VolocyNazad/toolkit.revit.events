using Microsoft.Extensions.DependencyInjection;
using Revit.Events.Abstractions.Services;
using Revit.Events.DI;
using Xunit;

namespace Revit.Events.Tests;

/// <summary>
///     Проверяет регистрацию сервисов пакета в DI-контейнере. Все тесты пропущены: <c>AddEvents()</c>
///     регистрирует <see cref="Revit.Events.Services.ExternalEvent"/> и
///     <see cref="Revit.Events.Services.AsyncExternalEvent"/> по их реализациям, а построение таблицы
///     виртуальных методов для этих типов (реализующих методы с параметром <c>Autodesk.Revit.UI.UIApplication</c>)
///     заставляет CLR грузить RevitAPIUI.dll и её нативные зависимости уже на этапе регистрации —
///     ещё до любого <c>GetRequiredService</c>. Вне установленного Revit это невозможно.
/// </summary>
public sealed class RegistratorTests
{
    private const string RevitRequiredSkipReason =
        "Требует установленного Revit: AddEvents() регистрирует ExternalEvent/AsyncExternalEvent, чьи методы " +
        "используют Autodesk.Revit.UI.UIApplication, поэтому уже загрузка этих типов для регистрации в DI " +
        "заставляет CLR резолвить RevitAPIUI.dll и её нативные зависимости, которых нет вне процесса/установки Revit.";

    [Fact(Skip = RevitRequiredSkipReason)]
    public void AddEvents_RegistersExternalEventAsSingleton()
    {
        ServiceCollection services = new();

        services.AddEvents();

        using var provider = services.BuildServiceProvider();

        var first = provider.GetRequiredService<IExternalEvent>();
        var second = provider.GetRequiredService<IExternalEvent>();

        Assert.Same(first, second);
    }

    [Fact(Skip = RevitRequiredSkipReason)]
    public void AddEvents_RegistersAsyncExternalEventAsSingleton()
    {
        ServiceCollection services = new();

        services.AddEvents();

        using var provider = services.BuildServiceProvider();

        var first = provider.GetRequiredService<IAsyncExternalEvent>();
        var second = provider.GetRequiredService<IAsyncExternalEvent>();

        Assert.Same(first, second);
    }

    [Fact(Skip = RevitRequiredSkipReason)]
    public void AddEvents_ReturnsSameServiceCollection_ForChaining()
    {
        ServiceCollection services = new();

        var result = services.AddEvents();

        Assert.Same(services, result);
    }
}
