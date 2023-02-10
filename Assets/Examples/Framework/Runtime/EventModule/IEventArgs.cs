
namespace AppFramework
{
    public interface IEventArgs
    {
        int EventDef { get; }
        object Sender { get; }
    }
}
