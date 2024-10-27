
namespace Example;

internal interface IExampleContravariant<in T>
{
    object? MyMethod(T arg);
}


internal interface IExampleDisposableContravariant<in T> : IExampleContravariant<T>, IDisposable, IAsyncDisposable
{

}

internal class ExampleService<T> : IExampleDisposableContravariant<T>
{
    public virtual object? MyMethod(T arg)
    {
        Console.WriteLine($"ExampleService<{typeof(T).Name}>.MyMethod({arg?.GetType().Name})");
        return arg;
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