using DispatchProxyAdvanced;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Reflection.Emit;

namespace ContravarianceDependencyInjection;

internal static class ServiceRegistrar
{
    public static void Register(IServiceCollection services, Type serviceType, SearchStrategy strategy, ServiceLifetime lifetime)
    {
        var serviceKey = $"variance_{Guid.NewGuid():N}";

        var proxyType = ProxyFactory.CreateType(serviceType,
            new CustomAttributeBuilder(_fromKeyedServicesAttributeCtor, [serviceKey]));

        var existGenericService = services.LastOrDefault(s => s.ServiceType == serviceType && !s.IsKeyedService && s.ImplementationType?.IsProxyType() == false);
        var existGenericImplementation = existGenericService?.ImplementationType;

        if (existGenericImplementation != null)
        {
            services.Add(new ServiceDescriptor(existGenericImplementation, serviceKey, existGenericImplementation, existGenericService!.Lifetime));
        }

        services.AddKeyedSingleton(serviceKey, (s, k) => new ServiceTypeAdapter(services, serviceType, strategy));

        services.AddKeyedTransient(serviceKey, (s, k) => ProxyHandlerFactory.Create(s, k, existGenericImplementation));

        services.Add(new ServiceDescriptor(serviceType, proxyType, lifetime));
    }


    static readonly ConstructorInfo _fromKeyedServicesAttributeCtor = typeof(FromKeyedServicesAttribute).GetConstructor([typeof(object)])!;
}
