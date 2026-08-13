using Microsoft.Extensions.DependencyInjection;
using Revit.Events.Abstractions.Services;
using Revit.Events.DI;
using Xunit;

namespace Revit.Events.Tests;

/// <summary>
///     Проверяет регистрацию сервисов пакета в DI-контейнере.
/// </summary>
public sealed class RegistratorTests
{
    [Fact]
    public void AddEvents_RegistersExternalEventAsSingleton()
    {
        ServiceCollection services = new();

        services.AddEvents();

        using var provider = services.BuildServiceProvider();

        var first = provider.GetRequiredService<IExternalEvent>();
        var second = provider.GetRequiredService<IExternalEvent>();

        Assert.Same(first, second);
    }

    [Fact]
    public void AddEvents_RegistersAsyncExternalEventAsSingleton()
    {
        ServiceCollection services = new();

        services.AddEvents();

        using var provider = services.BuildServiceProvider();

        var first = provider.GetRequiredService<IAsyncExternalEvent>();
        var second = provider.GetRequiredService<IAsyncExternalEvent>();

        Assert.Same(first, second);
    }

    [Fact]
    public void AddEvents_ReturnsSameServiceCollection_ForChaining()
    {
        ServiceCollection services = new();

        var result = services.AddEvents();

        Assert.Same(services, result);
    }
}
