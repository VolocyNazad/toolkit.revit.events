using Autodesk.Revit.UI;
using Revit.Events.Services;

namespace Revit.Events.Abstractions.Services;

/// <summary>
///     Synchronous invocation of a Revit external event: schedules an action to run inside
///     the Revit API context and immediately returns the result of queuing it.
/// </summary>
public interface IExternalEvent
{
    /// <summary>
    ///     Requests execution of <paramref name="action"/> in the Revit API context.
    /// </summary>
    /// <param name="action">The delegate executed with the active <see cref="UIApplication"/>.</param>
    /// <param name="options">Additional invocation options, see <see cref="ExternalEventOptions"/>.</param>
    /// <returns>The result of queuing the Revit event for execution.</returns>
    ExternalEventRequest Raise(Action<UIApplication> action, ExternalEventOptions options = ExternalEventOptions.None);
}
