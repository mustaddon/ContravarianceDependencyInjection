using DispatchProxyAdvanced;
using Microsoft.Extensions.DependencyInjection;

namespace ContravarianceDependencyInjection;

internal class ProxyHandlerFactory
{
    public static ProxyHandler Create(IServiceProvider services, object? serviceKey, Type serviceType, Type? defaultGenericServiceType)
    {
        var handler = CreateBase(services, serviceKey, defaultGenericServiceType);

        if (!typeof(IDisposable).IsAssignableFrom(serviceType))
            return handler;

        return (proxy, method, args) => 
        {
            if (method.DeclaringType == typeof(IDisposable))
                return null;

            return handler(proxy, method, args);
        };
    }

    static ProxyHandler CreateBase(IServiceProvider services, object? serviceKey, Type? defaultGenericServiceType)
    {
        var serviceTypeAdapter = services.GetRequiredKeyedService<ServiceTypeAdapter>(serviceKey);
        var serviceFactory = GetServiceFactory(services, serviceKey, defaultGenericServiceType);

        return (proxy, method, args) =>
        {
            var targetType = proxy.GetDeclaringType();

            var serviceType = serviceTypeAdapter.GetServiceType(targetType);

            var service = serviceFactory(targetType, serviceType);

            return method.Invoke(service, args);
        };
    }

    static Func<Type, Type?, object> GetServiceFactory(IServiceProvider services, object? serviceKey, Type? defaultGenericServiceType)
    {
        return (targetType, serviceType) =>
        {
            if (serviceType != null)
                return services.GetRequiredService(serviceType);

            if (defaultGenericServiceType != null)
                return services.GetRequiredKeyedService(
                    defaultGenericServiceType.MakeGenericType(targetType.GenericTypeArguments),
                    serviceKey);

            throw new InvalidOperationException($"Could not find a matching registered service for type '{targetType}'.");
        };
    }
}
