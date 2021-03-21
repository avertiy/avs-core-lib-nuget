using System;
using System.Threading.Tasks;

namespace AVS.CoreLib.WebSockets
{
    /// <summary>
    /// Represents base web socket client
    /// </summary>
    /// <example>
    /// PoloniexWebSocketClient : WebSocketClient
    /// {
    ///    override void OnMessageArrived(string message){...process messages...}
    ///    void Subscribe(..){
    ///        var command = ...
    ///        SendCommandAsync(command);
    ///    }
    /// }
    /// 
    /// using(var client = new PoloniexWebSocketClient("wss url"))
    /// {
    ///     client.Subscribe(..);
    /// }
    /// </example>
    public abstract class WebSocketClient : IDisposable
    {
        private readonly ISocketCommunicator _communicator;
        private bool _disposing;
        public bool IsConnected => _communicator.IsConnected;
        
        protected WebSocketClient(string wssApiUrl): this(new SocketCommunicator(wssApiUrl))
        {
        }

        protected WebSocketClient(ISocketCommunicator communicator)
        {
            _communicator = communicator;
            _communicator.ConnectionClosed += OnSocketConnectionClosed;
            _communicator.ConnectionError += OnSocketConnectionError;
            _communicator.MessageArrived += OnMessageArrived;
        }
        
        protected async Task SendCommandAsync(IChannelCommand command)
        {
            await _communicator.SendAsync(command);
        }
        
        protected virtual void OnMessageArrived(string message) { }

        protected virtual void OnSocketConnectionError(Exception obj)
        {
        }

        protected virtual async void OnSocketConnectionClosed()
        {
            if (!_disposing)
                await _communicator.ReconnectAsync();
        }

        public void Dispose()
        {
            _disposing = true;
            _communicator.Dispose();
        }
    }
}