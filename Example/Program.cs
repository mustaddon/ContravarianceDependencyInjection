using ContravarianceDependencyInjection;
using Example;
using Microsoft.Extensions.DependencyInjection;


// inheritance example Сontracts
// ContractBase -> ContractA -> ContractAB -> ContractABC
// ContractBase -> ContractB


var services = new ServiceCollection()
    // Contravariance test services
    .AddTransient<IExampleContravariant<ContractBase>, ExampleContravariant<ContractBase>>()
    .AddTransient<IExampleContravariant<ContractAB>, ExampleContravariant<ContractAB>>()
    .AddTransient<IExampleContravariant<ContractA>, ExampleContravariant<ContractA>>()
    // REQUIRED: Adds contravariance injection for registred IExampleContravariant services 
    .AddContravariance(typeof(IExampleContravariant<>), SearchStrategy.MaxCloser)

    // Covariance test services
    .AddTransient<IExampleCovariant<ContractABC>, ExampleCovariant<ContractABC>>()
    .AddContravariance(typeof(IExampleCovariant<>))

    // Both variance test services
    .AddTransient<IExampleBothvariant<ContractBase, ContractABC>, ExampleService<ContractBase, ContractABC>>()
    .AddTransient<IExampleBothvariant<ContractAB, ContractABC>, ExampleService<ContractAB, ContractABC>>()
    .AddTransient<IExampleBothvariant<ContractA, ContractAB>, ExampleService<ContractA, ContractAB>>()
    .AddContravariance(typeof(IExampleBothvariant<,>), SearchStrategy.MaxCloser)

    .BuildServiceProvider();




Console.WriteLine("\n=== Contravariance ===");

// IExampleContravariance<ContractABC> is not registered,
// IExampleContravariance<ContractAB> is invoked instead 
Console.WriteLine("  Result: " +
    services.GetRequiredService<IExampleContravariant<ContractABC>>()
        .MyContravariantMethod(new()));

// IExampleContravariance<ContractB> is not registered,
// IExampleContravariance<ContractBase> is invoked instead 
Console.WriteLine("  Result: " +
    services.GetRequiredService<IExampleContravariant<ContractB>>()
        .MyContravariantMethod(new()));




Console.WriteLine("\n=== Covariance ===");

// IExampleCovariant<ContractA> is not registered,
// IExampleCovariant<ContractABC> is invoked instead 
Console.WriteLine("  Result: " +
    services.GetRequiredService<IExampleCovariant<ContractA>>()
        .MyCovariantMethod(new()));




Console.WriteLine("\n=== Both variance ===");

// IExampleBothvariant<ContractABC, ContractA> is not registered,
// IExampleBothvariant<ContractAB, ContractABC> is invoked instead 
Console.WriteLine("  Result: " +
    services.GetRequiredService<IExampleBothvariant<ContractABC, ContractA>>()
        .MyMethod(new()));

// IExampleBothvariant<ContractB, ContractA> is not registered,
// IExampleBothvariant<ContractBase, ContractABC> is invoked instead 
Console.WriteLine("  Result: " +
    services.GetRequiredService<IExampleBothvariant<ContractB, ContractA>>()
        .MyMethod(new()));