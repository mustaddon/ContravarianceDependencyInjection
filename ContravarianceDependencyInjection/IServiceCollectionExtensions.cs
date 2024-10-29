using ContravarianceDependencyInjection;

namespace Microsoft.Extensions.DependencyInjection;

public static class ContravarianceDIServiceCollectionExtensions
{
    public static IServiceCollection AddContravariance(this IServiceCollection services,
        Type serviceType, 
        SearchStrategy strategy = SearchStrategy.LastRegistred)
    {
        if (!serviceType.IsInterface)
            throw new ArgumentException($"'{serviceType}' is not interface.");

        if (!serviceType.IsGenericTypeDefinition)
            throw new ArgumentException($"'{serviceType}' is not open generic type.");

        ServiceRegistrar.Register(services, serviceType, strategy);

        return services;
    }

}