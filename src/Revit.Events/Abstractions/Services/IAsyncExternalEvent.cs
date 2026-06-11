using Autodesk.Revit.UI;
using Revit.Events.Services;

namespace Revit.Events.Abstractions.Services;

public interface IAsyncExternalEvent
{
    Task Raise(Action<UIApplication> action, ExternalEventOptions options = ExternalEventOptions.None);
}
