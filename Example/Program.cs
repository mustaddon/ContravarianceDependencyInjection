using ContravarianceDependencyInjection;
using Example;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection()
    .AddTransient<IExampleContravariant<ContractBase>, ExampleService<ContractBase>>()
    .AddTransient<IExampleContravariant<ContractAB>, ExampleService<ContractAB>>()
    .AddTransient<IExampleContravariant<ContractA>, ExampleService<ContractA>>()

    // REQUIRED: Adds contravariance injection for registred IExampleContravariant services 
    .AddContravariance(typeof(IExampleContravariant<>), SearchStrategy.MaxCloser)

    .BuildServiceProvider();


// inheritance example Сontracts
// ContractBase -> ContractA -> ContractAB -> ContractABC
// ContractBase -> ContractB



// IExampleContravariance<ContractABC> is not registered,
// IExampleContravariance<ContractAB> is invoked instead 
Console.WriteLine("  Result: " +
    services.GetRequiredService<IExampleContravariant<ContractABC>>()
        .MyMethod(new ContractABC()));



// IExampleContravariance<ContractB> is not registered,
// IExampleContravariance<ContractBase> is invoked instead 
Console.WriteLine("  Result: " +
    services.GetRequiredService<IExampleContravariant<ContractB>>()
        .MyMethod(new ContractB()));