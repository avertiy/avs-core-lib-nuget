using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AVS.CoreLib.WebSockets
{
    /// <inheritdoc />
    public class SocketCommunicator : ISocketCommunicator
    {
        private ClientWebSocket _webSocket;
        private Uri _url;
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;
        private bool _disposing = false;
        private bool _backgroundTaskStarted = false;
        private readonly object _sync = new object();
        /// <summary>
        /// When enabled messages about connection closed/error etc. will be written by System.Diagnostics.Debug
        /// </summary>
        public bool DiagnosticEnabled { get; set; }

        public bool IsConnected => _webSocket.State == WebSocketState.Open && _backgroundTaskStarted;

        public async Task<bool> EnsureConnected()
        {
            start:
            if (_webSocket.State == WebSocketState.Open)
            {
                if(_backgroundTaskStarted)
                    return true;

                int i = 10;
                while (!_backgroundTaskStarted && i> 0)
                {
                    Thread.Sleep(10);
                    i--;
                }

                if (IsConnected)
                    return true;
            }
            await ConnectAsync();

            if (_disposing)
                return false;
            goto start;
        }

        /// <inheritdoc />
        public event Action<string> MessageArrived;
        public event Action ConnectionClosed;
        public event Action<Exception> ConnectionError;

        public SocketCommunicator(string uriString) :
            this(new Uri(uriString))
        {
        }

        public SocketCommunicator(Uri url)
        {
            _webSocket = new ClientWebSocket();
            _url = url;
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
        }

        public Uri Url
        {
            get => _url;
            set
            {
                if (value == null)
                    throw new SocketCommunicatorException("Url must be not null");
                _url = value;
            }
        }

        /// <inheritdoc />
        public async Task SendAsync(IChannelCommand command)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(command.ToJsonMessage());
            ArraySegment<byte> messageToSend = new ArraySegment<byte>(bytes);

            if (!IsConnected)
            {
                var connected = await EnsureConnected();
                if (!connected)
                    throw new SocketCommunicatorException("Not connected", $"[WebSocket state: {_webSocket.State}; disposing: {_disposing}]");
            }

            await _webSocket.SendAsync(messageToSend, WebSocketMessageType.Text, true, _cancellationToken);
        }
        /// <inheritdoc />
        public async Task ReconnectAsync()
        {
            if(_disposing)
                return;
            if (_webSocket.State == WebSocketState.Open)
                throw new InvalidOperationException("Connection state is Open");

            if (_cancellationTokenSource != null)
            {
                //cancel the task started when ConnectAsync was called
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }

            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;

            if (_webSocket != null && _webSocket.State == WebSocketState.Aborted)
            {
                _webSocket.Dispose();
                _webSocket = null;
                _webSocket = new ClientWebSocket();
            }

            await ConnectAsync();
        }
        
        /// <inheritdoc />
        public async Task ConnectAsync()
        {
            start:
            if(_disposing)
                return;
            if (_webSocket.State == WebSocketState.Connecting)
            {
                Thread.Sleep(10);
                goto start;
            }

            if (_webSocket.State != WebSocketState.Open)
            {
                try
                {
                    await _webSocket.ConnectAsync(_url, _cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    RaiseConnectionError(ex);
                }
            }

            if (!_backgroundTaskStarted)
                lock (_sync)
                {
                    if (!_backgroundTaskStarted)
                    {
                        _ = Task.Run(this.RunAsync, _cancellationToken);
                        _backgroundTaskStarted = true;
                    }
                }
        }

        /// <inheritdoc />
        protected virtual void RaiseMessageArrived(string message)
        {
            MessageArrived?.Invoke(message);
        }
        /// <inheritdoc />
        protected virtual void RaiseConnectionClosed()
        {
            if (DiagnosticEnabled)
                System.Diagnostics.Debug.Write("Connection has been closed", this.GetType().Name);
            ConnectionClosed?.Invoke();
        }
        /// <inheritdoc />
        protected virtual void RaiseConnectionError(Exception ex)
        {
            if (DiagnosticEnabled)
                System.Diagnostics.Debug.Write("A connection error occured: " + ex.Message, this.GetType().Name);
            ConnectionError?.Invoke(ex);
        }

        private async Task RunAsync()
        {
            try
            {
                /*We define a certain constant which will represent
                  size of received data. It is established by us and 
                  we can set any value. We know that in this case the size of the sent
                  data is very small.
                */
                const int maxMessageSize = 2048;

                // Buffer for received bits.
                ArraySegment<byte> receivedDataBuffer = new ArraySegment<byte>(new byte[maxMessageSize]);

                MemoryStream memoryStream = new MemoryStream();

                // Checks WebSocket state.
                while (IsConnected && !_cancellationToken.IsCancellationRequested)
                {
                    // Reads data.
                    WebSocketReceiveResult webSocketReceiveResult =
                        await ReadMessage(receivedDataBuffer, memoryStream).ConfigureAwait(false);

                    if (webSocketReceiveResult.MessageType != WebSocketMessageType.Close)
                    {
                        memoryStream.Position = 0;
                        OnNewMessage(memoryStream);
                    }
                    else if (webSocketReceiveResult.MessageType == WebSocketMessageType.Close)
                    {
                        await CloseWebSocket().ConfigureAwait(false);
                        break;
                    }

                    memoryStream.Position = 0;
                    memoryStream.SetLength(0);
                }
            }
            catch (Exception ex)
            {
                if (!(ex is OperationCanceledException) ||
                    !_cancellationToken.IsCancellationRequested)
                {
                    RaiseConnectionError(ex);
                }
            }
            
            if (_webSocket.State != WebSocketState.CloseReceived &&
                _webSocket.State != WebSocketState.Closed)
            {
                await CloseWebSocket().ConfigureAwait(false);
            }
            _backgroundTaskStarted = false;
            RaiseConnectionClosed();
        }

        private async Task CloseWebSocket()
        {
            _backgroundTaskStarted = false;
            if (_webSocket.State == WebSocketState.CloseReceived ||
                _webSocket.State == WebSocketState.Closed)
                return;

            try
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure,
                                            String.Empty,
                                            CancellationToken.None)
                                .ConfigureAwait(false);
                
            }
            catch (Exception ex)
            {
                if (DiagnosticEnabled)
                    System.Diagnostics.Debug.Write("CloseWebSocket failed: " + ex.Message, this.GetType().Name);
                //throw ex;
            }
        }

        private async Task<WebSocketReceiveResult> ReadMessage(ArraySegment<byte> receivedDataBuffer, MemoryStream memoryStream)
        {
            WebSocketReceiveResult webSocketReceiveResult;

            do
            {
                webSocketReceiveResult =
                    await _webSocket.ReceiveAsync(receivedDataBuffer, _cancellationToken)
                                    .ConfigureAwait(false);

                await memoryStream.WriteAsync(receivedDataBuffer.Array,
                                              receivedDataBuffer.Offset,
                                              webSocketReceiveResult.Count,
                                              _cancellationToken)
                                  .ConfigureAwait(false);
            }
            while (!webSocketReceiveResult.EndOfMessage);

            return webSocketReceiveResult;
        }

        private void OnNewMessage(MemoryStream payloadData)
        {
            var streamReader = new StreamReader(payloadData);
            var message = streamReader.ReadToEnd();

            if (DiagnosticEnabled)
                System.Diagnostics.Debug.Write("Received message: {Message} " + message, this.GetType().Name);

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
    }
}
