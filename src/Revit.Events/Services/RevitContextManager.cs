using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Revit.Events.Services;

internal sealed class RevitContextManager 
{
    private static readonly Func<bool> GetIsRevitInApiMode;
    private static readonly IntPtr IncrementConstructorPointer;
    private static readonly IntPtr IncrementDestructorPointer;

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

    public static Application Application { get; }
    public static UIApplication UiApplication { get; }

    public static bool IsRevitInApiMode => GetIsRevitInApiMode();

    public static IDisposable BeginApiContextScope() 
        => new RevitContextScope(IncrementConstructorPointer, IncrementDestructorPointer);

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

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0) return;

            var deconstructorDelegate = Marshal.GetDelegateForFunctionPointer<IncrementDtor>(_deconstructorPointer);
            deconstructorDelegate(_memory);

            Marshal.FreeHGlobal(_memory);
        }
    }

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