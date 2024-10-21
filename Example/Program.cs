using ContravarianceDependencyInjection;
using Example;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection()
    .AddTransient<IExampleContravariance<ContractBase>, ExampleService<ContractBase>>()
    .AddTransient<IExampleContravariance<ContractAB>, ExampleService<ContractAB>>()
    .AddTransient<IExampleContravariance<ContractA>, ExampleService<ContractA>>()

    // REQUIRED: Adds contravariance injection for registred services IExampleContravariance
    .AddContravariance(typeof(IExampleContravariance<>), SearchStrategy.MaxCloser)

    .BuildServiceProvider();


// inheritance example Сontracts
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