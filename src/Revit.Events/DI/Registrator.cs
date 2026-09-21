using Microsoft.Extensions.DependencyInjection;
using Revit.Events.Abstractions.Services;
using Revit.Events.Services;

namespace Revit.Events.DI;

/// <summary>
///     Extension methods for registering Revit.Events services in the dependency injection container.
/// </summary>
public static class Registrator
{
	extension(IServiceCollection services)
	{
        /// <summary>
        ///     Registers <see cref="IExternalEvent"/> and <see cref="IAsyncExternalEvent"/> as singleton services.
        /// </summary>
        /// <returns>The same service collection, for further chaining.</returns>
        public IServiceCollection AddEvents() => services
            .AddSingleton<IExternalEvent, ExternalEvent>()
            .AddSingleton<IAsyncExternalEvent, AsyncExternalEvent>()
       ;
    }
}
