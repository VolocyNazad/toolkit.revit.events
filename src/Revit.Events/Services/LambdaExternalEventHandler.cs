using Autodesk.Revit.UI;

namespace Revit.Events.Services;

/// <summary>
///     A wrapper over <see cref="Action{UIApplication}"/> that lets an arbitrary delegate be passed
///     as Revit API's <see cref="IExternalEventHandler"/>.
/// </summary>
/// <param name="action">The delegate invoked by Revit when the external event executes.</param>
internal sealed class LambdaExternalEventHandler(Action<UIApplication> action) : IExternalEventHandler
{
    private string? _name;
    private readonly Action<UIApplication> _action = action;

    /// <summary>
    ///     Invoked by the Revit API inside the Revit context, and delegates execution to the wrapped action.
    /// </summary>
    /// <param name="app">The active Revit application passed to the event handler.</param>
    public void Execute(UIApplication app) => _action(app);

    /// <summary>
    ///     Returns the unique handler name required by the Revit API. Generated once and cached.
    /// </summary>
    public string GetName() => _name ??= Guid.NewGuid().ToString();
}
