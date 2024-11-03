
namespace Example;

internal interface IExampleCovariant<out T>
{
    T MyCovariantMethod(object? arg);
}


internal interface IExampleDisposableCovariant<out T> : IExampleCovariant<T>, IDisposable, IAsyncDisposable
{

}