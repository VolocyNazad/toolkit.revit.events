using Microsoft.Extensions.DependencyInjection;
using Revit.Events.Abstractions.Services;
using Revit.Events.DI;
using Xunit;

namespace Revit.Events.Tests;

/// <summary>
///     Verifies registration of the package's services in the DI container. All tests are skipped: <c>AddEvents()</c>
///     registers <see cref="Revit.Events.Services.ExternalEvent"/> and
///     <see cref="Revit.Events.Services.AsyncExternalEvent"/> by their implementations, and building the
///     virtual method table for these types (implementing methods that take an <c>Autodesk.Revit.UI.UIApplication</c> parameter)
///     forces the CLR to load RevitAPIUI.dll and its native dependencies already at registration time —
///     before any <c>GetRequiredService</c> call. This is impossible outside of an installed Revit.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Test names use underscores to separate scenario and expected result.")]
public sealed class RegistratorTests
{
    private const string RevitRequiredSkipReason =
        "Requires Revit to be installed: AddEvents() registers ExternalEvent/AsyncExternalEvent, whose methods " +
        "use Autodesk.Revit.UI.UIApplication, so merely loading these types for DI registration " +
        "forces the CLR to resolve RevitAPIUI.dll and its native dependencies, which are unavailable outside of a Revit process/installation.";

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
