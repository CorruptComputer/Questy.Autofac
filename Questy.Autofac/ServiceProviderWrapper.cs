using Autofac;

namespace Questy.Autofac;

internal class ServiceProviderWrapper(ILifetimeScope lifeTimeScope) : IServiceProvider
{
    public object? GetService(Type serviceType) => lifeTimeScope.ResolveOptional(serviceType);
}