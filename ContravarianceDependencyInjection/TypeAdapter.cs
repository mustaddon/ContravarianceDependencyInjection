using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Reflection;

namespace ContravarianceDependencyInjection;

internal class TypeAdapter(IServiceCollection services, Type openType, SearchStrategy strategy)
{
    readonly Lazy<HashSet<Type>> _registred = new(() => new(GetFactoryTypes(services, openType, strategy)));
    readonly ConcurrentDictionary<Type, Lazy<Type?>> _found = new();

    public Type? FindServiceType(Type type)
    {
        return _found.GetOrAdd(type, (x) => new(() => CreateCacheValue(x))).Value;
    }

    Type? CreateCacheValue(Type type)
    {
        if (_registred.Value.Contains(type))
            return type;

        var candidates = _registred.Value.Where(type.IsAssignableFrom);

        if (strategy <= SearchStrategy.LastRegistred)
            return candidates.FirstOrDefault();

        candidates = candidates.ToList();

        var ranked = candidates.Select(x => new
        {
            Type = x,
            Rank = candidates.Where(x.IsAssignableFrom).Count()
        });

        ranked = strategy == SearchStrategy.MaxCloser
            ? ranked.OrderByDescending(x => x.Rank)
            : ranked.OrderBy(x => x.Rank);

        return ranked.Select(x => x.Type).FirstOrDefault();
    }

    static IEnumerable<Type> GetFactoryTypes(IServiceCollection services, Type openType, SearchStrategy matchingStrategy)
    {
        var types = GetFactoryTypes(services, openType);

        if (matchingStrategy == SearchStrategy.LastRegistred)
            return types.Reverse();

        return types;
    }

    static IEnumerable<Type> GetFactoryTypes(IServiceCollection services, Type openType)
    {
        foreach (var sd in services)
        {
            if (sd.IsKeyedService)
                continue;

            var type = sd.ServiceType;

            if (type.IsGenericType
                && !type.IsGenericTypeDefinition
                && type.GetGenericTypeDefinition() == openType)
                yield return type;
        }
    }
}
