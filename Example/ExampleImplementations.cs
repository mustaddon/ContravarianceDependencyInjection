
namespace Example;

internal class ExampleService<Tin, Tout> : 
    IExampleDisposableContravariant<Tin>, 
    IExampleDisposableCovariant<Tout>,
    IExampleBothvariant<Tin, Tout>
{
    public virtual object? MyContravariantMethod(Tin arg)
    {
        Console.WriteLine($"ExampleService<{typeof(Tin).Name}, {typeof(Tout).Name}>.MyContravariantMethod({arg?.GetType().Name})");
        return arg;
    }

    public Tout MyCovariantMethod(object? arg)
    {
        Console.WriteLine($"ExampleService<{typeof(Tin).Name}, {typeof(Tout).Name}>.MyCovariantMethod({arg?.GetType().Name})");
        return arg is Tout r ? r : Activator.CreateInstance<Tout>();
    }

    public Tout MyMethod(Tin arg)
    {
        Console.WriteLine($"ExampleService<{typeof(Tin).Name}, {typeof(Tout).Name}>.MyMethod({arg?.GetType().Name})");
        return arg is Tout r ? r : Activator.CreateInstance<Tout>();
    }

    public void Dispose()
    {
        Console.WriteLine($"Test Dispose: {this.GetType()}");
    }

    public ValueTask DisposeAsync()
    {
        Console.WriteLine($"Test DisposeAsync: {this.GetType()}");
        return ValueTask.CompletedTask;
    }
}