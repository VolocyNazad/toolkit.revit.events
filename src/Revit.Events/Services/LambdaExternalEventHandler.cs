using Autodesk.Revit.UI;

namespace Revit.Events.Services;

internal sealed class LambdaExternalEventHandler(Action<UIApplication> action) : IExternalEventHandler
{
    private string? _name;
    private readonly Action<UIApplication> _action = action;

    public void Execute(UIApplication app) => _action(app);

    public string GetName() => _name ??= Guid.NewGuid().ToString();
}
