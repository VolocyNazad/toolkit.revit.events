using Autodesk.Revit.UI;
using Revit.Events.Services;
using Xunit;

namespace Revit.Events.Tests;

/// <summary>
///     Verifies <see cref="LambdaExternalEventHandler"/>. All tests are skipped: the constructor takes an
///     <see cref="Action{UIApplication}"/>, and merely referencing the type's signature (without calling its members)
///     forces the CLR to load RevitAPIUI.dll and its native dependencies, which are unavailable outside of an installed Revit.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Test names use underscores to separate scenario and expected result.")]
public sealed class LambdaExternalEventHandlerTests
{
    private const string RevitRequiredSkipReason =
        "Requires Revit to be installed: LambdaExternalEventHandler's constructor takes Action<UIApplication>, " +
        "so even calling it forces the CLR to resolve RevitAPIUI.dll and its native dependencies, " +
        "which are unavailable outside of a Revit process/installation.";

    [Fact(Skip = RevitRequiredSkipReason)]
    public void GetName_ReturnsNonEmptyValue()
    {
        LambdaExternalEventHandler handler = new(_ => { });

        var name = handler.GetName();

        Assert.False(string.IsNullOrWhiteSpace(name));
    }

    [Fact(Skip = RevitRequiredSkipReason)]
    public void GetName_IsStableAcrossMultipleCalls()
    {
        LambdaExternalEventHandler handler = new(_ => { });

        var first = handler.GetName();
        var second = handler.GetName();

        Assert.Equal(first, second);
    }

    [Fact(Skip = RevitRequiredSkipReason)]
    public void Execute_InvokesUnderlyingAction()
    {
        // This test cannot run without Revit running — see the Skip description above.
    }
}
