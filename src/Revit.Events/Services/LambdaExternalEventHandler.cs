using Autodesk.Revit.UI;

namespace Revit.Events.Services;

/// <summary>
///     Обёртка над <see cref="Action{UIApplication}"/>, позволяющая передавать произвольный делегат
///     в качестве <see cref="IExternalEventHandler"/> Revit API.
/// </summary>
/// <param name="action">Делегат, вызываемый Revit при выполнении внешнего события.</param>
internal sealed class LambdaExternalEventHandler(Action<UIApplication> action) : IExternalEventHandler
{
    private string? _name;
    private readonly Action<UIApplication> _action = action;

    /// <summary>
    ///     Вызывается Revit API внутри контекста Revit и делегирует выполнение обёрнутому действию.
    /// </summary>
    /// <param name="app">Активное приложение Revit, переданное обработчику события.</param>
    public void Execute(UIApplication app) => _action(app);

    /// <summary>
    ///     Возвращает уникальное имя обработчика, требуемое Revit API. Генерируется один раз и кэшируется.
    /// </summary>
    public string GetName() => _name ??= Guid.NewGuid().ToString();
}
