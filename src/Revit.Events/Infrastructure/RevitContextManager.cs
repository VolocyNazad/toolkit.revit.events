using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Revit.Events.Infrastructure;

/// <summary>
///     Через рефлексию и низкоуровневый вызов внутренних методов Revit API получает доступ к текущему
///     <see cref="Application"/>/<see cref="UIApplication"/> и умеет определять/имитировать выполнение
///     кода внутри контекста Revit API (API-режим). Используется, чтобы вызывать действия напрямую,
///     минуя очередь внешних событий, когда это безопасно.
/// </summary>
internal sealed class RevitContextManager
{
    private static readonly Func<bool> GetIsRevitInApiMode;
    private static readonly IntPtr IncrementConstructorPointer;
    private static readonly IntPtr IncrementDestructorPointer;

    /// <summary>
    ///     Через рефлексию находит внутренние типы и методы сборок Revit API и подготавливает
    ///     делегаты/указатели, необходимые для работы <see cref="IsRevitInApiMode"/>
    ///     и <see cref="BeginApiContextScope"/>.
    /// </summary>
    static RevitContextManager()
    {
        var assemblies = FindAssemblies("RevitDBAPI", "APIUIAPI", "RevitAPIUI");

        var dbAssemblyMethods = assemblies[0].ManifestModule.GetMethods(BindingFlags.NonPublic | BindingFlags.Static);
        var getApplicationMethod = dbAssemblyMethods.FirstOrDefault(info => info.Name == "RevitApplication.getApplication_");

        var proxyType = assemblies[0].DefinedTypes.FirstOrDefault(info => info.FullName == "Autodesk.Revit.Proxy.ApplicationServices.ApplicationProxy");

        const BindingFlags internalFlags = BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.Instance;
        var proxyConstructor = proxyType.GetConstructor(internalFlags, null, [getApplicationMethod.ReturnType], null);

        var proxy = proxyConstructor.Invoke([getApplicationMethod.Invoke(null, null)]);

#if NET8_0_OR_GREATER
        Application = UnsafeAccessors.CreateApplication(proxy);
#else
        var applicationType = typeof(Application);
        var applicationConstructor = applicationType.GetConstructor(internalFlags, null, [proxyType], null);

        var application = (Application)applicationConstructor.Invoke([proxy]);

        Application = application;
#endif

        var apiAssemblyMethods = assemblies[1].ManifestModule.GetMethods(BindingFlags.NonPublic | BindingFlags.Static);
        var apiCallDepthManagerMethod = apiAssemblyMethods.FirstOrDefault(method => method.Name == "APICallDepthManager.singletonfactory");

        var isRevitInApiModeMethod = apiAssemblyMethods.FirstOrDefault(method => method.Name == "APICallDepthManager.isRevitInAPIMode");

        var uiAssemblyMethods = assemblies[2].ManifestModule.GetMethods(BindingFlags.NonPublic | BindingFlags.Static);
        var incrementConstructor = uiAssemblyMethods.FirstOrDefault(method => method.Name == "IncrementAPICallDepth.{ctor}");

        var incrementDestructor = uiAssemblyMethods.FirstOrDefault(method => method.Name == "IncrementAPICallDepth.{dtor}");

        IncrementConstructorPointer = incrementConstructor.MethodHandle.GetFunctionPointer();
        IncrementDestructorPointer = incrementDestructor.MethodHandle.GetFunctionPointer();

        GetIsRevitInApiMode = () =>
        {
            var apiCallDepthManager = apiCallDepthManagerMethod.Invoke(null, null);
            return (bool)isRevitInApiModeMethod.Invoke(null, [apiCallDepthManager])!;
        };

        UiApplication = new UIApplication(Application);
    }

    /// <summary>
    ///     Текущий экземпляр Revit <see cref="Application"/>, полученный через внутренний API Revit.
    /// </summary>
    public static Application Application { get; }

    /// <summary>
    ///     Обёртка <see cref="UIApplication"/> над <see cref="Application"/>.
    /// </summary>
    public static UIApplication UiApplication { get; }

    /// <summary>
    ///     Определяет, выполняется ли текущий вызов внутри контекста Revit API
    ///     (т. е. безопасно ли обращаться к объектам Revit API напрямую).
    /// </summary>
    public static bool IsRevitInApiMode => GetIsRevitInApiMode();

    /// <summary>
    ///     Открывает область, в которой Revit считает, что выполнение происходит внутри контекста API
    ///     (эмулирует вход в API-режим на время действия <see cref="IDisposable"/>).
    /// </summary>
    public static IDisposable BeginApiContextScope()
        => new RevitContextScope(IncrementConstructorPointer, IncrementDestructorPointer);

    /// <summary>
    ///     Область эмуляции API-режима Revit: увеличивает счётчик глубины API-вызова при создании
    ///     и уменьшает при <see cref="Dispose"/>.
    /// </summary>
    private sealed class RevitContextScope : IDisposable
    {
        private readonly IntPtr _memory;
        private readonly IntPtr _deconstructorPointer;
        private int _disposed;

        internal RevitContextScope(IntPtr constructorPointer, IntPtr deconstructorPointer)
        {
            _deconstructorPointer = deconstructorPointer;
            _memory = Marshal.AllocHGlobal(8);
            Marshal.WriteInt64(_memory, 0);

            var constructorDelegate = Marshal.GetDelegateForFunctionPointer<IncrementCtor>(constructorPointer);
            constructorDelegate(_memory);
        }

        /// <summary>
        ///     Закрывает область: уменьшает счётчик глубины API-вызова и освобождает неуправляемую память.
        /// </summary>
        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0) return;

            var deconstructorDelegate = Marshal.GetDelegateForFunctionPointer<IncrementDtor>(_deconstructorPointer);
            deconstructorDelegate(_memory);

            Marshal.FreeHGlobal(_memory);
        }
    }

    /// <summary>
    ///     Находит среди загруженных в текущий домен сборок те, чьё простое имя входит в <paramref name="names"/>.
    /// </summary>
    /// <param name="names">Искомые простые имена сборок.</param>
    /// <returns>Найденные сборки в том же порядке, что и <paramref name="names"/>.</returns>
    private static Assembly[] FindAssemblies(params string[] names)
    {
        HashSet<string> remaining = new(names);
        Dictionary<string, Assembly> result = new(names.Length);

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            var name = assembly.GetName().Name;
            if (name is not null && remaining.Remove(name))
            {
                result[name] = assembly;
                if (remaining.Count == 0) break;
            }
        }

        return names.Select(name => result[name]).ToArray();
    }

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    private delegate IntPtr IncrementCtor(IntPtr self);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    private delegate void IncrementDtor(IntPtr self);
}