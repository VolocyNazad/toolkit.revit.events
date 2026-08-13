using Microsoft.Extensions.DependencyInjection;
using Revit.Events.Abstractions.Services;
using Revit.Events.Services;

namespace Revit.Events.DI;

/// <summary>
///     Методы расширения для регистрации сервисов Revit.Events в контейнере зависимостей.
/// </summary>
public static class Registrator
{
	extension(IServiceCollection services)
	{
        /// <summary>
        ///     Регистрирует <see cref="IExternalEvent"/> и <see cref="IAsyncExternalEvent"/> как singleton-сервисы.
        /// </summary>
        /// <returns>Та же коллекция сервисов для дальнейшей цепочки вызовов.</returns>
        public IServiceCollection AddEvents() => services
            .AddSingleton<IExternalEvent, ExternalEvent>()
            .AddSingleton<IAsyncExternalEvent, AsyncExternalEvent>()
       ;
    }
}
