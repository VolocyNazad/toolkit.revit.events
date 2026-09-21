using Revit.Events.Services;
using Xunit;

namespace Revit.Events.Tests;

/// <summary>
///     Verifies the behavior of the <see cref="ExternalEventOptions"/> flags — pure logic with no Revit API access.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Test names use underscores to separate scenario and expected result.")]
public sealed class ExternalEventOptionsTests
{
    [Fact]
    public void None_HasNoFlagsSet()
    {
        Assert.False(ExternalEventOptions.None.HasFlag(ExternalEventOptions.AllowDirectInvocation));
    }

    [Fact]
    public void AllowDirectInvocation_IsRecognizedAsFlag()
    {
        var options = ExternalEventOptions.AllowDirectInvocation;

        Assert.True(options.HasFlag(ExternalEventOptions.AllowDirectInvocation));
    }

    [Fact]
    public void CombiningWithNone_DoesNotChangeValue()
    {
        var options = ExternalEventOptions.AllowDirectInvocation | ExternalEventOptions.None;

        Assert.Equal(ExternalEventOptions.AllowDirectInvocation, options);
    }
}
