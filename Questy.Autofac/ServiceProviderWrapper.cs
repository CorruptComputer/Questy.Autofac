using Autofac;

namespace Questy.Autofac;

internal class ServiceProviderWrapper : IServiceProvider
{
    private readonly ILifetimeScope lifeTimeScope;

    public ServiceProviderWrapper(ILifetimeScope lifeTimeScope)
    {
        this.lifeTimeScope = lifeTimeScope;
    }

    public object? GetService(Type serviceType) => this.lifeTimeScope.ResolveOptional(serviceType);
}