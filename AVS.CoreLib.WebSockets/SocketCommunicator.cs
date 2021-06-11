using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AVS.CoreLib.WebSockets.Abstractions;
using AVS.CoreLib.WebSockets.Extensions;

namespace AVS.CoreLib.WebSockets
{
    public class SocketCommunicator : ISocketCommunicator
    {
        private bool _disposing = false;
        private ClientWebSocket _webSocket;
        public WebSocketState State => _webSocket.State;

        public SocketCommunicator()
        {
            _webSocket = new ClientWebSocket();
        }

        public async Task<bool> ConnectAsync(Uri uri, CancellationToken cancellationToken)
        {
            if (_disposing)
                return false;
            if (State == WebSocketState.Open)
                return true;

            await _webSocket.ConnectAsync(uri, cancellationToken)
                .ConfigureAwait(false);

            _ = RunReceiveMessages(cancellationToken);

            WaitWebSocketConnecting();
            return State == WebSocketState.Open;
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


        public async Task SendAsync(string commandMessage, CancellationToken cancellationToken)
        {
            if(_disposing)
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
            while (State == WebSocketState.Connecting)
            {
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
                        await ReceiveMessagesLoopAsync(cancellationToken).ConfigureAwait(false);
                        FireConnectionClosed();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                FireConnectionError(ex);
            }
        }

        private async Task ReceiveMessagesLoopAsync(CancellationToken cancellationToken)
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

            //var i = 1;
            // Check WebSocket state.
            while (State == WebSocketState.Open && !_disposing)
            {
                if (cancellationToken.IsCancellationRequested)
                {
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
                    break;
                }

                var message = await ReadMessageAsync(memoryStream);
                FireMessageArrived(message);

                //if (i++ % 100 == 0)
                //{
                //    //emulate closed connection
                //    throw new SocketCommunicatorException("test exception");
                //}
            }
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
        public event Action ConnectionClosed;
        public event Action<Exception> ConnectionError;

        /// <summary>
        /// Fire FireMessageArrived event
        /// </summary>
        protected virtual void FireMessageArrived(string message)
        {
            MessageArrived?.Invoke(message);
        }

        /// <summary>
        /// Fire ConnectionClosed event
        /// </summary>
        protected virtual void FireConnectionClosed()
        {
            ConnectionClosed?.Invoke();
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
    }

    /*
    public class SocketCommunicatorOld //: ISocketCommunicator
    {
        private readonly ILogger _logger;
        private ClientWebSocket _webSocket;
        private Uri _uri;
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;
        private bool _disposing = false;
        private bool _receiveMessagesLoopIsRunning = false;
        private readonly object _sync = new object();
        /// <summary>
        /// When enabled messages about connection closed/error etc. will be written by System.Diagnostics.Debug
        /// </summary>
        public bool DiagnosticEnabled { get; set; }

        public bool IsConnected => _webSocket.State == WebSocketState.Open && _receiveMessagesLoopIsRunning;

        public WebSocketState State => _webSocket.State;

        /// <inheritdoc />
        public event Action<string> MessageArrived;
        public event Action ConnectionClosed;
        public event Action<Exception> ConnectionError;

        public Uri Url
        {
            get => _uri;
            set
            {
                if (value == null)
                    throw new SocketCommunicatorException("Url must be not null");
                _uri = value;
            }
        }

        public SocketCommunicator(ILogger logger)
        {
            _logger = logger;
            _webSocket = new ClientWebSocket();
            //_uri = new Uri(uriString);
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
        }

        /// <summary>
        /// Ensure connected and send command message to web socket channel
        /// </summary>
        public async Task SendAsync(IChannelCommand command)
        {
            var message = command.ToJsonMessage();
            var bytes = Encoding.UTF8.GetBytes(message);
            var messageToSend = new ArraySegment<byte>(bytes);

            if (!IsConnected)
            {
                var connected = await EnsureConnected();
                if (!connected)
                {
                    _logger.LogError("{class}::{method}: web socket connection failed [state: {state}; background task: {task}]",
                        nameof(SocketCommunicator),
                        nameof(SendAsync),
                        State,
                        _receiveMessagesLoopIsRunning);
                    throw new SocketCommunicatorException("Not connected",
                        $"[state: {State}; background task: {_disposing}]");
                }
            }

            if (DiagnosticEnabled)
            {
                _logger.LogInformation(
                    "{class}::{method}: sending message {message} to {url} [state: {state}; background task: {task}]",
                    nameof(SocketCommunicator),
                    nameof(SendAsync),
                    message,
                    Url,
                    State,
                    _receiveMessagesLoopIsRunning);
            }

            await _webSocket.SendAsync(messageToSend, WebSocketMessageType.Text, true, _cancellationToken);
        }

        /// <summary>
        /// connect web socket to uri channel and start background task to receive messages
        /// </summary>
        public async Task<bool> EnsureConnected(int attempts = 3)
        {
            if (IsConnected)
                return true;

            do
            {
                await ConnectAsync();
                if (IsConnected)
                    return true;
                Thread.Sleep(10);
            } while (attempts-- > 0);

            return false;
        }

        /// <summary>
        /// connect web socket to Url and start background task to read messages
        /// </summary>
        public async Task<bool> ConnectAsync()
        {
            if (State == WebSocketState.None)
            {
                if (DiagnosticEnabled)
                {
                    _logger.LogInformation("{class}::{method}: connecting to {url} [state: {state}]",
                        nameof(SocketCommunicator), nameof(ConnectAsync), Url, State);
                }

                try
                {
                    await _webSocket.ConnectAsync(Url, _cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "{class}::{method}: connect to {url} failed [state: {state}]. Raising ConnectionError event.",
                        nameof(SocketCommunicator), nameof(ConnectAsync), Url, State);
                    return false;
                }
            }

            WaitWhileConnecting();

            if (DiagnosticEnabled)
            {
                _logger.LogInformation("{class}::{method}: connected [state: {state}]",
                    nameof(SocketCommunicator), nameof(ConnectAsync), State);
            }

            RunReceiveMessagesTask();
            return true;
        }

        private void RunReceiveMessagesTask()
        {
            if (State != WebSocketState.Open)
                throw new SocketCommunicatorException($"Open state expected [state: {State}]");

            if (!_receiveMessagesLoopIsRunning)
                lock (_sync)
                {
                    if (!_receiveMessagesLoopIsRunning && State == WebSocketState.Open)
                    {
                        if (DiagnosticEnabled)
                        {
                            _logger.LogInformation(
                                "{class}::{method}: starting background task",
                                nameof(SocketCommunicator), nameof(RunReceiveMessagesTask));
                        }

                        _ = Task.Run(this.BackgroundTask, _cancellationToken);

                        WaitWhileReceiveMessagesTaskStarting();
                        if (DiagnosticEnabled)
                        {
                            _logger.LogInformation(
                                "{class}::{method}: background task is started",
                                nameof(SocketCommunicator), nameof(RunReceiveMessagesTask));
                        }
                    }
                }
        }

        /// <summary>
        /// reconnect web socket
        /// </summary>
        public async Task<bool> ReconnectAsync()
        {
            if (_disposing)
                return false;
            if (State == WebSocketState.Open)
                throw new InvalidOperationException("Connection state is Open");

            _logger.LogInformation("{class}::{method}: reconnecting [state: {state}]",
                nameof(SocketCommunicator), nameof(ReconnectAsync), State);

            if (_cancellationTokenSource != null)
            {
                //cancel the task started when ConnectAsync was called
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }

            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;

            if (_webSocket != null && (State == WebSocketState.Aborted || State == WebSocketState.Closed))
            {
                _webSocket.Dispose();
                _webSocket = null;
                _webSocket = new ClientWebSocket();
                if (DiagnosticEnabled)
                {
                    _logger.LogInformation("{class}::{method} WebSocket disposed and new one created [state: {state}]",
                        nameof(SocketCommunicator),
                        nameof(ReconnectAsync), 
                        State);
                }
            }

            return await EnsureConnected();
        }

        private void WaitWhileConnecting()
        {
            start:
            if (State == WebSocketState.Connecting)
            {
                if (DiagnosticEnabled)
                {
                    _logger.LogInformation("{class}::{method}: waiting connect to {url}  [state: {state}]",
                        nameof(SocketCommunicator), nameof(ConnectAsync), Url, State);
                }
                Thread.Sleep(10);
                goto start;
            }
        }

        private void WaitWhileReceiveMessagesTaskStarting()
        {
            var i = 10;
            while (!_receiveMessagesLoopIsRunning && i > 0)
            {
                Thread.Sleep(10);
                i--;
            }
        }

        

        /// <summary>
        /// Raises FireMessageArrived event
        /// </summary>
        protected virtual void RaiseMessageArrived(string message)
        {
            MessageArrived?.Invoke(message);
        }

        /// <summary>
        /// Raises ConnectionClosed event
        /// </summary>
        protected virtual void RaiseConnectionClosed()
        {
            ConnectionClosed?.Invoke();
        }
        /// <summary>
        /// Raises ConnectionError event
        /// </summary>
        protected virtual void RaiseConnectionError(Exception ex)
        {
            ConnectionError?.Invoke(ex);
        }

        private async Task BackgroundTask()
        {
            try
            {
                await ReceiveMessagesLoopAsync();
            }
            catch (WebSocketException ex)
            {
                _logger.LogError(ex,
                    "{class}::{method}: web socket error. Raising ConnectionError event.",
                    nameof(SocketCommunicator), nameof(BackgroundTask));
                RaiseConnectionError(ex);
            }
            catch (Exception ex)
            {
                if (_cancellationToken.IsCancellationRequested)
                {
                    _logger.LogError(ex, "{class}::{method}: !!!!! cancellation requested !!!!!", nameof(SocketCommunicator), nameof(BackgroundTask));
                }
                else
                {
                    _logger.LogError(ex,
                        "{class}::{method}: failed. Raising ConnectionError event.",
                        nameof(SocketCommunicator), nameof(BackgroundTask));
                    RaiseConnectionError(ex);
                }
            }

        }

        private async Task ReceiveMessagesLoopAsync()
        {
            _receiveMessagesLoopIsRunning = true;

            
            const int maxMessageSize = 2048;

            // Buffer for received bits.
            var receivedDataBuffer = new ArraySegment<byte>(new byte[maxMessageSize]);

            var memoryStream = new MemoryStream();
            try
            {
                // Check WebSocket state.
                while (IsConnected)
                {
                    // Reads data.
                    var webSocketReceiveResult = await _webSocket.GetWebSocketReceiveResultAsync(
                        memoryStream, receivedDataBuffer, _cancellationToken).ConfigureAwait(false);

                    if (webSocketReceiveResult.MessageType == WebSocketMessageType.Close)
                    {
                        if (DiagnosticEnabled)
                        {
                            _logger.LogInformation(
                                "{class}::{method}: WebSocket Close message received",
                                nameof(SocketCommunicator), nameof(ReceiveMessagesLoopAsync));
                        }

                        await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure,
                                string.Empty,
                                CancellationToken.None)
                            .ConfigureAwait(false);
                        _receiveMessagesLoopIsRunning = false;
                        RaiseConnectionClosed();
                        break;
                    }

                    await ReadMessageAsync(memoryStream);

                    if (_cancellationToken.IsCancellationRequested)
                    {
                        if (DiagnosticEnabled)
                        {
                            _logger.LogInformation("{class}::{method}: cancellation requested",
                                nameof(SocketCommunicator),
                                nameof(ReceiveMessagesLoopAsync));
                        }

                        break;
                    }

                    memoryStream.Position = 0;
                    memoryStream.SetLength(0);
                }
            }
            catch (OperationCanceledException ex)
            {
                if (DiagnosticEnabled)
                {
                    _logger.LogInformation(ex,
                        "{class}::{method}: operation canceled exception",
                        nameof(SocketCommunicator), nameof(BackgroundTask));
                }
            }
            finally
            {
                _receiveMessagesLoopIsRunning = false;
            }
        }

        private async Task ReadMessageAsync(MemoryStream memoryStream)
        {
            memoryStream.Position = 0;
            var streamReader = new StreamReader(memoryStream);
            var message = await streamReader.ReadToEndAsync();
            if (DiagnosticEnabled)
            {
                _logger.LogTrace("{class}::{method}: WebSocket message received {message}", nameof(SocketCommunicator), nameof(ReadMessageAsync), message);
            }
            RaiseMessageArrived(message);
        }
        /// <inheritdoc />
        public void Dispose()
        {
            _disposing = true;
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
            _webSocket.Dispose();
            _webSocket = null;
        }
    }*/
}
