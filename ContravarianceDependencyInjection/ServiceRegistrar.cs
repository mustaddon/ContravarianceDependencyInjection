using DispatchProxyAdvanced;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection.Emit;

namespace ContravarianceDependencyInjection;

internal static class ServiceRegistrar
{
    public static void Register(IServiceCollection services, Type serviceType, SearchStrategy strategy, ServiceLifetime lifetime)
    {
        var serviceKey = string.Concat(KEY_PREFIX, Guid.NewGuid().ToString("N"));

        var proxyType = ProxyFactory.CreateType(serviceType,
            new CustomAttributeBuilder(typeof(FromKeyedServicesAttribute).GetConstructor([typeof(object)])!, [serviceKey]));

        var existGenericService = services.LastOrDefault(s => s.ServiceType == serviceType && !s.IsKeyedService && s.ImplementationType != null && !s.ImplementationType.IsContravarianceDI());
        var existGenericImplementation = existGenericService?.ImplementationType;

        if (existGenericImplementation != null)
        {
            services.Add(new ServiceDescriptor(existGenericImplementation, serviceKey, existGenericImplementation, existGenericService!.Lifetime));
        }

        services.AddKeyedSingleton(serviceKey, (s, k) => new ServiceTypeAdapter(services, serviceType, strategy));

        services.AddKeyedTransient(serviceKey, (s, k) => ProxyHandlerFactory.Create(s, k, serviceType, existGenericImplementation));

        services.Add(new ServiceDescriptor(serviceType, proxyType, lifetime));
    }

    internal const string KEY_PREFIX = "ContravarianceDI_";
}
