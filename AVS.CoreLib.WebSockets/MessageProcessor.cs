using System.Threading;
using System.Threading.Tasks;
using AVS.CoreLib.WebSockets.Abstractions;

namespace AVS.CoreLib.WebSockets
{
    public class MessageProcessor: IMessageProcessor
    {
        private bool _processMessageAsyncEnabled = true;
        public bool IsDebug { get; set; }

        public virtual void ProcessMessage(string message)
        {
            // avoid overhead of Task.Run if ProcessMessageAsync was not overriden
            if (!_processMessageAsyncEnabled)
                return;

            if (IsDebug)
            {
                ProcessMessageAsync(message);
            }
            else
            {
                Task.Run(() => ProcessMessageAsync(message), CancellationToken.None);
            }
        }

        protected virtual Task ProcessMessageAsync(string message)
        {
            // if method was not overriden switch off ProcessMessageAsync
            _processMessageAsyncEnabled = false;
            return Task.CompletedTask;
        }
    }
}