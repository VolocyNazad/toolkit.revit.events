using Autodesk.Revit.UI;
using Revit.Events.Abstractions.Services;
using Revit.Events.Infrastructure;

namespace Revit.Events.Services;

/// <summary>
///     Реализация <see cref="IExternalEvent"/> поверх штатного механизма
///     <see cref="Autodesk.Revit.UI.ExternalEvent"/> Revit API.
/// </summary>
internal sealed class ExternalEvent : IExternalEvent
{
    /// <inheritdoc />
    public ExternalEventRequest Raise(Action<UIApplication> action, ExternalEventOptions options = ExternalEventOptions.None)
    {
        if ((options & ExternalEventOptions.AllowDirectInvocation) != 0 && RevitContextManager.IsRevitInApiMode)
        {
            action(RevitContextManager.UiApplication);
            return ExternalEventRequest.Accepted;
        }

        IExternalEventHandler handler = new LambdaExternalEventHandler(action);

        Autodesk.Revit.UI.ExternalEvent externalEvent;
        using (RevitContextManager.BeginApiContextScope())
        {
            externalEvent = Autodesk.Revit.UI.ExternalEvent.Create(handler);
        }
        return externalEvent.Raise();
    }
}
