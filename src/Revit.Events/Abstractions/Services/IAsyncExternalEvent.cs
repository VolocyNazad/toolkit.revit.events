using Autodesk.Revit.UI;
using Revit.Events.Services;

namespace Revit.Events.Abstractions.Services;

/// <summary>
///     Asynchronous invocation of a Revit external event: schedules an action to run inside
///     the Revit API context and returns a <see cref="Task"/> that completes once the action has finished.
/// </summary>
public interface IAsyncExternalEvent
{
    /// <summary>
    ///     Requests execution of <paramref name="action"/> in the Revit API context and awaits its completion.
    /// </summary>
    /// <param name="action">The delegate executed with the active <see cref="UIApplication"/>.</param>
    /// <param name="options">Additional invocation options, see <see cref="ExternalEventOptions"/>.</param>
    /// <returns>
    ///     A task that completes after <paramref name="action"/> has run;
    ///     transitions to a faulted state if the delegate threw an exception.
    /// </returns>
    Task Raise(Action<UIApplication> action, ExternalEventOptions options = ExternalEventOptions.None);
}
