using Revit.Events.Services;
using Xunit;

namespace Revit.Events.Tests;

/// <summary>
///     Проверяет поведение флагов <see cref="ExternalEventOptions"/> — чистая логика без обращения к Revit API.
/// </summary>
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
