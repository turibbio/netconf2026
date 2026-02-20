using Microsoft.Extensions.DependencyInjection;
using TUnit.Core;
using TUnit.Core.Enums;
using TUnit.Core.Interfaces;

namespace TUnitDemo;

/// <summary>
/// ClassConstructor che usa IServiceProvider per iniettare le dipendenze
/// nelle classi di test TUnit tramite constructor injection.
/// </summary>
public class DependencyInjectionClassConstructor : IClassConstructor, ITestEndEventReceiver
{
    private static readonly IServiceProvider ServiceProvider = CreateServiceProvider();
    private AsyncServiceScope _scope;

    public Task<object> Create(Type type, ClassConstructorMetadata classConstructorMetadata)
    {
        _scope = ServiceProvider.CreateAsyncScope();
        return Task.FromResult(
            ActivatorUtilities.GetServiceOrCreateInstance(_scope.ServiceProvider, type));
    }

    public EventReceiverStage Stage => EventReceiverStage.Late;

    public ValueTask OnTestEnd(TestContext context)
    {
        return _scope.DisposeAsync();
    }

    private static IServiceProvider CreateServiceProvider()
    {
        return new ServiceCollection()
            .AddSingleton<IUrlShortener, UrlShortener>()
            .BuildServiceProvider();
    }
}
