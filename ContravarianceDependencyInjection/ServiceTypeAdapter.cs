﻿using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Reflection;

namespace ContravarianceDependencyInjection;

internal class ServiceTypeAdapter(IServiceCollection services, Type openType, SearchStrategy strategy)
{
    readonly Lazy<HashSet<Type>> _registred = new(() => new(GetFactoryTypes(services, openType, strategy)));
    readonly ConcurrentDictionary<Type, Lazy<Type?>> _found = new();

    public Type? GetServiceType(Type type)
    {
        return _found.GetOrAdd(type, (x) => new(() => FindValue(x))).Value;
    }

    Type? FindValue(Type type)
    {
        if (_registred.Value.Contains(type))
            return type;

        var candidates = _registred.Value.Where(type.IsAssignableFrom);

        if (strategy <= SearchStrategy.LastRegistred)
            return candidates.FirstOrDefault();

        var candidatesList = candidates.ToList();

        if (candidatesList.Count < 2)
            return candidatesList.FirstOrDefault();

        var ranked = candidatesList.Select(x => new
        {
            Type = x,
            Rank = candidatesList.Where(x.IsAssignableFrom).Count()
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

            if (type.IsInterface
                && type.IsGenericType
                && !type.IsGenericTypeDefinition
                && type.GetGenericTypeDefinition() == openType)
                yield return type;
        }
    }
}
