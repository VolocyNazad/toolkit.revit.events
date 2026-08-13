#if NET8_0_OR_GREATER
using System.Runtime.CompilerServices;
using Autodesk.Revit.ApplicationServices;

namespace Revit.Events.Infrastructure;

/// <summary>
///     Небезопасные аксессоры к внутренним (недоступным через рефлексию на .NET 8+) членам Revit API.
/// </summary>
internal static class UnsafeAccessors
{
    /// <summary>
    ///     Вызывает внутренний конструктор <see cref="Application"/>, минуя ограничения доступа.
    /// </summary>
    /// <param name="proxy">Экземпляр внутреннего прокси-объекта Revit API, передаваемый в конструктор.</param>
    [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
    internal static extern Application CreateApplication(object proxy);
}
#endif
