using System;
using System.Diagnostics;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AVS.CoreLib.WebSockets.Abstractions;
using AVS.CoreLib.WebSockets.Extensions;

namespace AVS.CoreLib.WebSockets
{
    /// <summary>
    ///  An abstraction layer over <see cref=" System.Net.WebSockets.ClientWebSocket"/>
    ///  SocketCommunicator encapsulates the necessary routine:
    ///   - opening web socket connection,
    ///   - managing web socket state,
    ///   - receiving messages loop in a background thread
    ///   - firing events: ConnectionClosed, ConnectionError, MessageArrived / MessageArrivedAsync (async one is invoked with Task.Run to avoid blocking the listening thread) 
    /// </summary>
    public class SocketCommunicator : ISocketCommunicator
    {
        private bool _disposing = false;
        private ClientWebSocket _webSocket;

        public WebSocketState State => _webSocket.State;

        /// <summary>
        /// timeout in milliseconds
        /// </summary>
        public int ConnectionTimeout { get; set; } = 180000;

        public bool IsBackgroundTaskActive { get; private set; }

        /// <summary>
        /// Shortcut to <see cref="ClientWebSocketOptions.KeepAliveInterval"/>
        /// Default keep alive interval is 30 sec. <see cref="WebSocket.DefaultKeepAliveInterval"/>
        /// </summary>
        public TimeSpan KeepAliveInterval
        {
            get => _webSocket.Options.KeepAliveInterval;
            set => _webSocket.Options.KeepAliveInterval = value;
        }

        public ClientWebSocketOptions Options => _webSocket.Options;

        public SocketCommunicator()
        {
            _webSocket = new ClientWebSocket();
        }

        public async Task<bool> ConnectAsync(Uri uri, CancellationToken ct)
        {
            start:
            if (_disposing)
                return false;

            if (State == WebSocketState.Open)
                return true;

            // parallel connection has been started  
            if (State == WebSocketState.Connecting)
            {
                WaitWebSocketConnecting();
                goto start;
            }

            await _webSocket.ConnectAsync(uri, ct)
                .ConfigureAwait(false);

            _ = RunReceiveMessages(ct);


            return State == WebSocketState.Open;
        }

        public async Task SendAsync(string commandMessage, CancellationToken cancellationToken)
        {
            if (_disposing)
                return;

            var bytes = Encoding.UTF8.GetBytes(commandMessage);
            var messageToSend = new ArraySegment<byte>(bytes);

            WaitWebSocketConnecting();
            if (State != WebSocketState.Open)
                throw new SocketCommunicatorException($"State must be Open [state: {State}]");

            await _webSocket.SendAsync(messageToSend, WebSocketMessageType.Text, true, cancellationToken);
        }

        public void ResetWebSocket()
        {
            if (_webSocket != null)
            {
                _webSocket.Dispose();
                _webSocket = null;
                _webSocket = new ClientWebSocket();
            }
        }

        private void WaitWebSocketConnecting()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            while (State == WebSocketState.Connecting)
            {
                if (stopwatch.ElapsedMilliseconds > ConnectionTimeout)
                    throw new SocketCommunicatorException($"WebSocket connection timeout [web socket state: {State}]");

                Thread.Sleep(10);
            }
        }

        private Task RunReceiveMessages(CancellationToken cancellationToken)
        {
            return Task.Run(() => this.BackgroundTask(cancellationToken), cancellationToken);
        }

        private async Task BackgroundTask(CancellationToken cancellationToken)
        {
            try
            {
                start:
                switch (State)
                {
                    case WebSocketState.Connecting:
                        WaitWebSocketConnecting();
                        goto start;
                    case WebSocketState.Open:
                        {
                            var reason = await ReceiveMessagesLoopAsync(cancellationToken).ConfigureAwait(false);
                            FireConnectionClosed(reason);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                FireConnectionError(ex);
            }
        }

        private async Task<string> ReceiveMessagesLoopAsync(CancellationToken cancellationToken)
        {
            /*We define a certain constant which will represent
              size of received data. It is established by us and 
              we can set any value. We know that in this case the size of the sent
              data is very small.
            */
            const int maxMessageSize = 2048;

            // Buffer for received bits.
            var receivedDataBuffer = new ArraySegment<byte>(new byte[maxMessageSize]);

            var memoryStream = new MemoryStream();
            string reason = null;
            IsBackgroundTaskActive = true;
            //var i = 1;
            // Check WebSocket state.
            while (true)
            {

                if (_disposing)
                {
                    reason = "Disposing";
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    reason = "CancellationRequested";
                    await CloseAsync();
                    break;
                }

                memoryStream.Position = 0;
                memoryStream.SetLength(0);

                // Receive web socket message
                var webSocketReceiveResult = await _webSocket.GetWebSocketReceiveResultAsync(
                    memoryStream, receivedDataBuffer, cancellationToken).ConfigureAwait(false);

                if (webSocketReceiveResult.MessageType == WebSocketMessageType.Close)
                {
                    await CloseAsync();
                    reason = "MessageTypeClose";
                    break;
                }

                var message = await ReadMessageAsync(memoryStream);
                FireMessageArrived(message);


                if (State != WebSocketState.Open)
                {
                    reason = $"WebSocketState:{State}";
                    break;
                }

                //if (i++ % 100 == 0)
                //{
                //    //emulate closed connection
                //    throw new SocketCommunicatorException("test exception");
                //}
            }

            IsBackgroundTaskActive = false;

            return reason;
        }

        private async Task CloseAsync()
        {
            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure,
                    string.Empty,
                    CancellationToken.None)
                .ConfigureAwait(false);
        }

        private async Task<string> ReadMessageAsync(MemoryStream memoryStream)
        {
            memoryStream.Position = 0;
            var streamReader = new StreamReader(memoryStream);
            return await streamReader.ReadToEndAsync();
        }

        #region events 

        public event Action<string> MessageArrived;
        public event Action<string> MessageArrivedAsync;
        public event Action<string> ConnectionClosed;
        public event Action<Exception> ConnectionError;

        /// <summary>
        /// Fire FireMessageArrived event
        /// </summary>
        protected virtual void FireMessageArrived(string message)
        {
            if (MessageArrivedAsync != null)
            {
                Task.Run(() => MessageArrivedAsync.Invoke(message));
            }
            else
            {
                MessageArrived?.Invoke(message);
            }
        }

        /// <summary>
        /// Fire ConnectionClosed event
        /// </summary>
        protected virtual void FireConnectionClosed(string reason)
        {
            ConnectionClosed?.Invoke(reason);
        }

        /// <summary>
        /// Fire ConnectionError event
        /// </summary>
        protected virtual void FireConnectionError(Exception ex)
        {
            ConnectionError?.Invoke(ex);
        }
        #endregion

        public void Dispose()
        {
            _disposing = true;
            _webSocket.Dispose();
            _webSocket = null;
        }

        //public async Task Test(Uri uri, string commandMessage, CancellationToken cancellationToken)
        //{
        //    await ConnectAsync(uri, cancellationToken).ConfigureAwait(false);
        //    Console.WriteLine($"WebSocket state:{State}");
        //    await SendAsync(commandMessage, cancellationToken).ConfigureAwait(false);
        //    Console.WriteLine($"command has been sent");
        //    Console.WriteLine($"Closing web socket");
        //    Thread.Sleep(2500);
        //    await CloseAsync().ConfigureAwait(false);
        //    Thread.Sleep(100);
        //    _webSocket.Dispose();
        //    Console.WriteLine($"web socket is Disposed!");
        //    _webSocket = new ClientWebSocket();
        //    Console.WriteLine($"New web socket created");
        //    await ConnectAsync(uri, cancellationToken).ConfigureAwait(false);
        //    Console.WriteLine($"WebSocket state:{State}");
        //    await SendAsync(commandMessage, cancellationToken).ConfigureAwait(false);
        //    Thread.Sleep(2500);
        //    Console.WriteLine($"Closing web socket");
        //    await CloseAsync().ConfigureAwait(false);
        //    Console.WriteLine($"web socket closed!");
        //    Thread.Sleep(500);
        //    _webSocket.Dispose();
        //    Console.WriteLine($"web socket is Disposed!");
        //    _webSocket = new ClientWebSocket();
        //    Console.WriteLine($"New web socket created");
        //}
    }
}
