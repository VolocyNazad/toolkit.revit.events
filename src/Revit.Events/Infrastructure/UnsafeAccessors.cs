#if NET8_0_OR_GREATER
using System.Runtime.CompilerServices;
using Autodesk.Revit.ApplicationServices;

namespace Revit.Events.Infrastructure;

/// <summary>
///     Unsafe accessors for internal Revit API members (not reachable via reflection on .NET 8+).
/// </summary>
internal static class UnsafeAccessors
{
    /// <summary>
    ///     Invokes the internal <see cref="Application"/> constructor, bypassing access restrictions.
    /// </summary>
    /// <param name="proxy">The internal Revit API proxy object instance passed to the constructor.</param>
    [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
    internal static extern Application CreateApplication(object proxy);
}
#endif
