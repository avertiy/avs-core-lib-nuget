using System.Threading;
using System.Threading.Tasks;
using AVS.CoreLib.WebSockets.Abstractions;

namespace AVS.CoreLib.WebSockets.MessageProcessing
{
    public abstract class MessageHandler<T, TContext> : IMessageHandler<T, TContext>
    {
        protected bool ProcessAsync { get; set; } = true;
        /// <summary>
        /// Set IsDebug true to handle messages in a sync way
        /// by default for each message Task.Run(() => HandleMessageAsync(...)) is used 
        /// </summary>
        public bool IsDebug { get; set; }

        public void Handle(T message, TContext context = default)
        {
            if(Filter(message))
                return;

            if (ProcessAsync)
            {
                if (IsDebug)
                    HandleMessageAsync(message, context);
                else
                    Task.Run(() => HandleMessageAsync(message, context), CancellationToken.None);
            }
            else
            {
                HandleMessage(message, context);
            }
        }

        protected virtual void HandleMessage(T message, TContext context = default)
        {   
        }

        protected virtual Task HandleMessageAsync(T message, TContext context)
        {
            // if method was not overriden switch off ProcessMessageAsync
            ProcessAsync = false;
            return Task.CompletedTask;
        }

        protected virtual bool Filter(T message)
        {
            return Equals(message, default(T));
        }
    }
}