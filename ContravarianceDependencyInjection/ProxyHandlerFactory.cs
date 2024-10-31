using DispatchProxyAdvanced;
using Microsoft.Extensions.DependencyInjection;

namespace ContravarianceDependencyInjection;

internal class ProxyHandlerFactory
{
    public static ProxyHandler Create(IServiceProvider services, object? serviceKey, Type serviceType)
    {
        var handler = CreateBase(services, serviceKey);

        if (!typeof(IDisposable).IsAssignableFrom(serviceType))
            return handler;

        return (proxy, method, args) =>
        {
            if (method.DeclaringType == typeof(IDisposable))
                return null;

            return handler(proxy, method, args);
        };
    }

    static ProxyHandler CreateBase(IServiceProvider services, object? serviceKey)
    {
        return (proxy, method, args) => method.Invoke(proxy.GetOrAddState(() =>
        {
            var targetType = proxy.GetDeclaringType();

            var serviceType = services
                .GetRequiredKeyedService<ServiceTypeAdapter>(serviceKey)
                .GetServiceType(targetType);

            return serviceType != null
                ? services.GetRequiredService(serviceType)
                : services.GetRequiredKeyedService(targetType, serviceKey);
        }), args);
    }
}
