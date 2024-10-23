using DispatchProxyAdvanced;
using Microsoft.Extensions.DependencyInjection;

namespace ContravarianceDependencyInjection;

internal class ProxyHandlerFactory
{
    public static ProxyHandler Create(IServiceProvider services, object? serviceKey, Type? defaultGenericServiceType)
    {
        var serviceTypeAdapter = services.GetRequiredKeyedService<ServiceTypeAdapter>(serviceKey);
        var serviceFactory = GetServiceFactory(services, serviceKey, defaultGenericServiceType);

        return new ProxyHandler((proxy, method, args) =>
        {
            var currentType = proxy.GetDeclaringType();

            var serviceType = serviceTypeAdapter.GetServiceType(currentType);

            var service = serviceFactory(currentType, serviceType);

            return method.Invoke(service, args);
        });
    }

    static Func<Type, Type?, object> GetServiceFactory(IServiceProvider services, object? serviceKey, Type? defaultGenericServiceType)
    {
        if (defaultGenericServiceType != null)
            return (targetType, serviceType) =>
            {
                if (serviceType != null)
                    return services.GetRequiredService(serviceType);

                return services.GetRequiredKeyedService(defaultGenericServiceType
                    .MakeGenericType(targetType.GenericTypeArguments), serviceKey);
            };


        return (targetType, serviceType) =>
        {
            if (serviceType == null)
                throw new InvalidOperationException($"Could not find a matching registered service for type '{targetType}'.");

            return services.GetRequiredService(serviceType);
        };
    }
}
