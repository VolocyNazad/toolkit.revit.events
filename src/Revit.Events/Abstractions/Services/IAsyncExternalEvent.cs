using Autodesk.Revit.UI;
using Revit.Events.Services;

namespace Revit.Events.Abstractions.Services;

/// <summary>
///     Асинхронный запуск внешнего события Revit: планирует выполнение <paramref name="action"/> внутри
///     контекста Revit API и возвращает <see cref="Task"/>, завершающийся после отработки действия.
/// </summary>
public interface IAsyncExternalEvent
{
    /// <summary>
    ///     Запрашивает выполнение <paramref name="action"/> в контексте Revit API и дожидается его завершения.
    /// </summary>
    /// <param name="action">Делегат, выполняемый с активным <see cref="UIApplication"/>.</param>
    /// <param name="options">Дополнительные параметры запуска, см. <see cref="ExternalEventOptions"/>.</param>
    /// <returns>
    ///     Задача, завершающаяся после выполнения <paramref name="action"/>;
    ///     переходит в состояние ошибки, если делегат выбросил исключение.
    /// </returns>
    Task Raise(Action<UIApplication> action, ExternalEventOptions options = ExternalEventOptions.None);
}
