# ContravarianceDependencyInjection [![NuGet version](https://badge.fury.io/nu/ContravarianceDependencyInjection.svg?1)](http://badge.fury.io/nu/ContravarianceDependencyInjection)
Contravariance extensions to IServiceCollection.

### Example
```C#
var services = new ServiceCollection()
    .AddTransient<IExampleContravariance<ContractBase>, ExampleService<ContractBase>>()
    .AddTransient<IExampleContravariance<ContractAB>, ExampleService<ContractAB>>()
    .AddTransient<IExampleContravariance<ContractA>, ExampleService<ContractA>>()

    // REQUIRED: Adds contravariance injection for registred services IExampleContravariance
    .AddContravariance(typeof(IExampleContravariance<>), SearchStrategy.MaxCloser)

    .BuildServiceProvider();


// inheritance example contracts
// ContractBase -> ContractA -> ContractAB -> ContractABC
// ContractBase -> ContractB



// IExampleContravariance<ContractABC> is not registered,
// IExampleContravariance<ContractAB> is invoked instead 
Console.WriteLine("  Result: " +
    services.GetRequiredService<IExampleContravariance<ContractABC>>()
        .MyMethod(new ContractABC()));



// IExampleContravariance<ContractB> is not registered,
// IExampleContravariance<ContractBase> is invoked instead 
Console.WriteLine("  Result: " +
    services.GetRequiredService<IExampleContravariance<ContractB>>()
        .MyMethod(new ContractB()));
```

[Program.cs](https://github.com/mustaddon/ContravarianceDependencyInjection/blob/main/Example/Program.cs)
