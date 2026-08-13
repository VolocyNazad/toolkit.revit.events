namespace Revit.Events.Services;

/// <summary>
///     Флаги, управляющие поведением <see cref="Abstractions.Services.IExternalEvent"/>
///     и <see cref="Abstractions.Services.IAsyncExternalEvent"/> при постановке события Revit в очередь.
/// </summary>
[Flags]
public enum ExternalEventOptions
{
    /// <summary>
    ///     Поведение по умолчанию: действие всегда выполняется через штатный механизм внешних событий Revit.
    /// </summary>
    None = 0,

    /// <summary>
    ///     Если вызов уже выполняется внутри контекста Revit API, действие выполняется немедленно,
    ///     минуя постановку в очередь внешних событий.
    /// </summary>
    AllowDirectInvocation = 1 << 0
}
