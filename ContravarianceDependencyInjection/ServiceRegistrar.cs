using DispatchProxyAdvanced;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection.Emit;

namespace ContravarianceDependencyInjection;

internal static class ServiceRegistrar
{
    public static void Register(IServiceCollection services, Type serviceType, SearchStrategy strategy)
    {
        var serviceKey = string.Concat(KEY_PREFIX, Guid.NewGuid().ToString("N"));

        var proxyType = ProxyFactory.CreateType(serviceType,
            new CustomAttributeBuilder(typeof(FromKeyedServicesAttribute).GetConstructor([typeof(object)])!, [serviceKey]));

        var existGenericService = services.LastOrDefault(s => s.ServiceType == serviceType && !s.IsKeyedService && s.ImplementationType != null && !s.ImplementationType.IsContravarianceDI());

        if (existGenericService != null)
            services.Add(new ServiceDescriptor(serviceType, serviceKey, existGenericService.ImplementationType!, existGenericService.Lifetime));

        services.AddKeyedSingleton(serviceKey, (s, k) => new ServiceTypeAdapter(services, serviceType, strategy));

        services.AddKeyedTransient(serviceKey, (s, k) => ProxyHandlerFactory.Create(s, k, serviceType));

        services.AddTransient(serviceType, proxyType);
    }

    internal const string KEY_PREFIX = "ContravarianceDI_";
}
