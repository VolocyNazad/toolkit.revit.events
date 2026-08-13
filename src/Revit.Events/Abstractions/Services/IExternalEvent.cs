using Autodesk.Revit.UI;
using Revit.Events.Services;

namespace Revit.Events.Abstractions.Services;

/// <summary>
///     Синхронный запуск внешнего события Revit: планирует выполнение <paramref name="action"/> внутри
///     контекста Revit API и сразу возвращает результат постановки в очередь.
/// </summary>
public interface IExternalEvent
{
    /// <summary>
    ///     Запрашивает выполнение <paramref name="action"/> в контексте Revit API.
    /// </summary>
    /// <param name="action">Делегат, выполняемый с активным <see cref="UIApplication"/>.</param>
    /// <param name="options">Дополнительные параметры запуска, см. <see cref="ExternalEventOptions"/>.</param>
    /// <returns>Результат постановки события Revit в очередь на выполнение.</returns>
    ExternalEventRequest Raise(Action<UIApplication> action, ExternalEventOptions options = ExternalEventOptions.None);
}
