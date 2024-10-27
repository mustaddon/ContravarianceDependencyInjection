using DispatchProxyAdvanced;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ContravarianceDependencyInjection;

public static class TypeExtensions
{
    public static bool IsContravarianceDI(this Type type)
    {
        return (type.GetProxyHandlerParameter()
            ?.GetCustomAttribute<FromKeyedServicesAttribute>()
            ?.Key as string)
            ?.StartsWith(ServiceRegistrar.KEY_PREFIX) == true;
    }
}
