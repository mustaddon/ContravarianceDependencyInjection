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
        var serviceTypeAdapter = services.GetRequiredKeyedService<ServiceTypeAdapter>(serviceKey);

        return (proxy, method, args) =>
        {
            var targetType = proxy.GetDeclaringType();

            var serviceType = serviceTypeAdapter.GetServiceType(targetType);

            var service = serviceType != null
                ? services.GetRequiredService(serviceType)
                : services.GetRequiredKeyedService(targetType, serviceKey);

            return method.Invoke(service, args);
        };
    }
}
