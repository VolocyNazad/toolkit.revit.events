using Autodesk.Revit.UI;
using Revit.Events.Services;
using Xunit;

namespace Revit.Events.Tests;

/// <summary>
///     Проверяет <see cref="LambdaExternalEventHandler"/>. Часть тестов пропущена: конструирование
///     реального объекта <see cref="UIApplication"/> вне процесса Revit невозможно.
/// </summary>
public sealed class LambdaExternalEventHandlerTests
{
    [Fact]
    public void GetName_ReturnsNonEmptyValue()
    {
        LambdaExternalEventHandler handler = new(_ => { });

        var name = handler.GetName();

        Assert.False(string.IsNullOrWhiteSpace(name));
    }

    [Fact]
    public void GetName_IsStableAcrossMultipleCalls()
    {
        LambdaExternalEventHandler handler = new(_ => { });

        var first = handler.GetName();
        var second = handler.GetName();

        Assert.Equal(first, second);
    }

    [Fact(Skip = "Требует установленного Revit: создание/использование реального UIApplication заставляет CLR " +
                 "грузить RevitAPI/RevitAPIUI, а они не запускаются вне процесса/установки Revit.")]
    public void Execute_InvokesUnderlyingAction()
    {
        // Тест не может быть выполнен без запущенного Revit — см. описание Skip выше.
    }
}
