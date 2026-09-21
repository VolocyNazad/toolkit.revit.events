using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Revit.Events.Infrastructure;

/// <summary>
///     Uses reflection and low-level calls into internal Revit API methods to obtain access to the current
///     <see cref="Application"/>/<see cref="UIApplication"/> and to detect/emulate execution of
///     code inside the Revit API context (API mode). Used to invoke actions directly,
///     bypassing the external event queue, when it is safe to do so.
/// </summary>
internal sealed class RevitContextManager
{
    private static readonly Func<bool> GetIsRevitInApiMode;
    private static readonly IntPtr IncrementConstructorPointer;
    private static readonly IntPtr IncrementDestructorPointer;

    /// <summary>
    ///     Uses reflection to locate the internal types and methods in the Revit API assemblies and prepares
    ///     the delegates/pointers required for <see cref="IsRevitInApiMode"/>
    ///     and <see cref="BeginApiContextScope"/> to work.
    /// </summary>
    static RevitContextManager()
    {
        var assemblies = FindAssemblies("RevitDBAPI", "APIUIAPI", "RevitAPIUI");

        var dbAssemblyMethods = assemblies[0].ManifestModule.GetMethods(BindingFlags.NonPublic | BindingFlags.Static);
        var getApplicationMethod = dbAssemblyMethods.FirstOrDefault(info => info.Name == "RevitApplication.getApplication_")
            ?? throw new MissingMethodException("RevitDBAPI", "RevitApplication.getApplication_");

        var proxyType = assemblies[0].DefinedTypes.FirstOrDefault(info => info.FullName == "Autodesk.Revit.Proxy.ApplicationServices.ApplicationProxy")
            ?? throw new TypeLoadException("Autodesk.Revit.Proxy.ApplicationServices.ApplicationProxy was not found.");

        const BindingFlags internalFlags = BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.Instance;
        var proxyConstructor = proxyType.GetConstructor(internalFlags, null, [getApplicationMethod.ReturnType], null)
            ?? throw new MissingMethodException(proxyType.FullName, ".ctor");

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
        var apiCallDepthManagerMethod = apiAssemblyMethods.FirstOrDefault(method => method.Name == "APICallDepthManager.singletonfactory")
            ?? throw new MissingMethodException("APIUIAPI", "APICallDepthManager.singletonfactory");

        var isRevitInApiModeMethod = apiAssemblyMethods.FirstOrDefault(method => method.Name == "APICallDepthManager.isRevitInAPIMode")
            ?? throw new MissingMethodException("APIUIAPI", "APICallDepthManager.isRevitInAPIMode");

        var uiAssemblyMethods = assemblies[2].ManifestModule.GetMethods(BindingFlags.NonPublic | BindingFlags.Static);
        var incrementConstructor = uiAssemblyMethods.FirstOrDefault(method => method.Name == "IncrementAPICallDepth.{ctor}")
            ?? throw new MissingMethodException("RevitAPIUI", "IncrementAPICallDepth.{ctor}");

        var incrementDestructor = uiAssemblyMethods.FirstOrDefault(method => method.Name == "IncrementAPICallDepth.{dtor}")
            ?? throw new MissingMethodException("RevitAPIUI", "IncrementAPICallDepth.{dtor}");

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
    ///     The current Revit <see cref="Application"/> instance, obtained via Revit's internal API.
    /// </summary>
    public static Application Application { get; }

    /// <summary>
    ///     A <see cref="UIApplication"/> wrapper over <see cref="Application"/>.
    /// </summary>
    public static UIApplication UiApplication { get; }

    /// <summary>
    ///     Determines whether the current call is running inside the Revit API context
    ///     (i.e. whether it is safe to access Revit API objects directly).
    /// </summary>
    public static bool IsRevitInApiMode => GetIsRevitInApiMode();

    /// <summary>
    ///     Opens a scope in which Revit believes execution is happening inside the API context
    ///     (emulates entering API mode for the lifetime of the returned <see cref="IDisposable"/>).
    /// </summary>
    public static IDisposable BeginApiContextScope()
        => new RevitContextScope(IncrementConstructorPointer, IncrementDestructorPointer);

    /// <summary>
    ///     A scope emulating Revit's API mode: increments the API call depth counter on creation
    ///     and decrements it on <see cref="Dispose"/>.
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
        ///     Closes the scope: decrements the API call depth counter and frees unmanaged memory.
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
    ///     Finds, among the assemblies loaded into the current domain, those whose simple name is in <paramref name="names"/>.
    /// </summary>
    /// <param name="names">The simple assembly names to look for.</param>
    /// <returns>The found assemblies, in the same order as <paramref name="names"/>.</returns>
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
