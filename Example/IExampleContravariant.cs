
namespace Example;

internal interface IExampleContravariant<in T>
{
    object? MyContravariantMethod(T arg);
}


internal interface IExampleDisposableContravariant<in T> : IExampleContravariant<T>, IDisposable, IAsyncDisposable
{

}