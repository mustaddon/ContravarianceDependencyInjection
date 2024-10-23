namespace Example;

internal interface IExampleContravariant<in T>
{
    object? MyMethod(T arg);
}


internal class ExampleService<T> : IExampleContravariant<T>
{
    public virtual object? MyMethod(T arg)
    {
        Console.WriteLine($"ExampleService<{typeof(T).Name}>.MyMethod({arg?.GetType().Name})");
        return arg;
    }
}