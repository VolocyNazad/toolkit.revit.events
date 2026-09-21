namespace Revit.Events.Services;

/// <summary>
///     Flags controlling the behavior of <see cref="Abstractions.Services.IExternalEvent"/>
///     and <see cref="Abstractions.Services.IAsyncExternalEvent"/> when queuing a Revit event.
/// </summary>
[Flags]
public enum ExternalEventOptions
{
    /// <summary>
    ///     Default behavior: the action always runs through Revit's standard external event mechanism.
    /// </summary>
    None = 0,

    /// <summary>
    ///     If the call is already running inside the Revit API context, the action executes immediately,
    ///     bypassing the external event queue.
    /// </summary>
    AllowDirectInvocation = 1 << 0
}
