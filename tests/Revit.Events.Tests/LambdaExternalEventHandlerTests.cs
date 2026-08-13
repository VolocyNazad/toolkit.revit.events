using Autodesk.Revit.UI;
using Revit.Events.Services;
using Xunit;

namespace Revit.Events.Tests;

/// <summary>
///     Проверяет <see cref="LambdaExternalEventHandler"/>. Все тесты пропущены: конструктор принимает
///     <see cref="Action{UIApplication}"/>, и уже само обращение к сигнатуре типа (без вызова его членов)
///     заставляет CLR грузить RevitAPIUI.dll и её нативные зависимости, недоступные вне установленного Revit.
/// </summary>
public sealed class LambdaExternalEventHandlerTests
{
    private const string RevitRequiredSkipReason =
        "Требует установленного Revit: конструктор LambdaExternalEventHandler принимает Action<UIApplication>, " +
        "поэтому даже его вызов заставляет CLR резолвить RevitAPIUI.dll и её нативные зависимости, " +
        "которых нет вне процесса/установки Revit.";

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
        // Тест не может быть выполнен без запущенного Revit — см. описание Skip выше.
    }
}
