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

        var existServiceDescriptor = services.LastOrDefault(s => s.ServiceType == serviceType && !s.IsKeyedService);
        var existServiceImplementationType = existServiceDescriptor?.ImplementationType;

        if (existServiceImplementationType != null)
        {
            services.Add(new ServiceDescriptor(existServiceImplementationType, serviceKey, existServiceImplementationType, existServiceDescriptor!.Lifetime));
        }

        services.AddKeyedSingleton(serviceKey, (s, k) => new TypeAdapter(services, serviceType, strategy));

        services.AddKeyedTransient(serviceKey, (s, k) => ServiceAdapterFactory.Create(s, k, existServiceImplementationType));

        services.Add(new ServiceDescriptor(serviceType, proxyType, lifetime));
    }


    static readonly ConstructorInfo _fromKeyedServicesAttributeCtor = typeof(FromKeyedServicesAttribute).GetConstructor([typeof(object)])!;
}
