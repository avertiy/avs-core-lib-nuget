namespace AVS.CoreLib.WebSockets.Abstractions
{
    /// <summary>
    /// message handler interface to handle web socket messages
    /// </summary>
    public interface IMessageHandler<in T, in TContext>
    {
        void Handle(T message, TContext context = default);
    }
}