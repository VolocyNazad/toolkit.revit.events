using Microsoft.Extensions.DependencyInjection;
using Revit.Events.Abstractions.Services;
using Revit.Events.Services;

namespace Revit.Events.DI;

public static class Registrator
{
	extension(IServiceCollection services)
	{
        public IServiceCollection AddEvents() => services
            .AddSingleton<IExternalEvent, ExternalEvent>()
            .AddSingleton<IAsyncExternalEvent, AsyncExternalEvent>()
       ;
    }
}
