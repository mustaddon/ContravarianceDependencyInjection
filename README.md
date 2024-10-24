# ContravarianceDependencyInjection [![NuGet version](https://badge.fury.io/nu/ContravarianceDependencyInjection.svg?7)](http://badge.fury.io/nu/ContravarianceDependencyInjection)
Contravariance extensions to IServiceCollection.

### Example
```C#
var services = new ServiceCollection()
    .AddTransient<IExampleContravariant<ContractBase>, ExampleService<ContractBase>>()
    .AddTransient<IExampleContravariant<ContractAB>, ExampleService<ContractAB>>()
    .AddTransient<IExampleContravariant<ContractA>, ExampleService<ContractA>>()

    // REQUIRED: Adds contravariance injection for registred services IExampleContravariant
    .AddContravariance(typeof(IExampleContravariant<>), SearchStrategy.MaxCloser)

    .BuildServiceProvider();


// inheritance example contracts
// ContractBase -> ContractA -> ContractAB -> ContractABC
// ContractBase -> ContractB


// IExampleContravariant<ContractABC> is not registered,
// IExampleContravariant<ContractAB> is invoked instead 
Console.WriteLine("  Result: " +
    services.GetRequiredService<IExampleContravariant<ContractABC>>()
        .MyMethod(new ContractABC()));


// IExampleContravariant<ContractB> is not registered,
// IExampleContravariant<ContractBase> is invoked instead 
Console.WriteLine("  Result: " +
    services.GetRequiredService<IExampleContravariant<ContractB>>()
        .MyMethod(new ContractB()));
```

[Program.cs](https://github.com/mustaddon/ContravarianceDependencyInjection/blob/main/Example/Program.cs)
