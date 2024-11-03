
namespace Example;

internal interface IExampleBothvariant<in Tin, out Tout>
{
    Tout MyMethod(Tin arg);
}


internal interface IExampleDisposableBothvariant<in Tin, out Tout> : IExampleBothvariant<Tin, Tout>, IDisposable, IAsyncDisposable
{

}