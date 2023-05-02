using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using AVS.CoreLib.WebSockets.Abstractions;
using Microsoft.Extensions.Logging;

namespace AVS.CoreLib.WebSockets
{
    /// <summary> 
    /// WebSocketClient represent the 2nd layer of abstraction over <see cref="System.Net.WebSockets.ClientWebSocket"/>
    /// It is based on <see cref="ISocketCommunicator"/> which is the 1st layer of abstraction.
    /// The idea is to provide a more simple interface encapsulating routine for reconnection, sending a command/message, disposing websocket and handling the communicator events.
    /// The client wrap socket communicator events forwarding only <see cref="MessageArrived"/> and <see cref="ConnectionClosed"/>.
    /// Also when <see cref="DiagnosticEnabled"/> is set to true enables logging that helps to identify websocket communication issues
    /// </summary>
    public class WebSocketClient : IWebSocketClient
    {
        private Uri _uri;
        protected ISocketCommunicator _communicator;
        protected readonly ILogger _logger;
        protected CancellationTokenSource _cancellationTokenSource;
        protected readonly CancellationToken _cancellationToken;

        #region props
        protected bool IsDisposing { get; set; }

        public bool DiagnosticEnabled { get; set; }

        public WebSocketState State => _communicator.State;

        public bool IsConnected => State == WebSocketState.Open;

        /// <summary>
        /// shortcut to <see cref="ClientWebSocketOptions.KeepAliveInterval"/>
        /// </summary>
        public TimeSpan KeepAliveInterval
        {
            get => _communicator.Options.KeepAliveInterval;
            set => _communicator.Options.KeepAliveInterval = value;
        }

        public ClientWebSocketOptions Options => _communicator.Options;


        private Uri Uri
        {
            get => _uri == null ? throw new InvalidOperationException("Uri was not initialized") : _uri;
            set => _uri = value;
        }

        #endregion

        #region events
        public event Action<string> ConnectionClosed;
        public event Action<string> MessageArrived; 
        #endregion

        public WebSocketClient(ILogger logger): this(new SocketCommunicator(), logger)
        {
        }

        public WebSocketClient(ISocketCommunicator communicator, ILogger logger)
        {
            _communicator = communicator;
            _logger = logger;
            _communicator.ConnectionClosed += OnSocketConnectionClosed;
            _communicator.ConnectionError += OnSocketConnectionError;
            _communicator.MessageArrived += OnMessageArrived;
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
        }

        public async Task SendAsync(string url, string command)
        {
            Uri = _uri == null || State is WebSocketState.Closed or WebSocketState.None
                ? new Uri(url)
                : throw new ArgumentException($"WebSocket state {State} does not allow to switch uri. Multiple websocket connections not supported yet.");

            await EnsureConnected().ConfigureAwait(false);
            await SendCommandAsync(command).ConfigureAwait(false);
        }

        protected async Task<bool> EnsureConnected()
        {
            if (State == WebSocketState.Open)
                return true;

            if (DiagnosticEnabled)
                _logger.LogInformation("{class}::{method}: web socket connecting to {uri} [state: {state}]", nameof(WebSocketClient), nameof(EnsureConnected), Uri, State);

            try
            {
                return await _communicator.ConnectAsync(Uri, _cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{class}::{method}: connect to {url} failed [state: {state}]",
                    nameof(WebSocketClient), nameof(EnsureConnected), Uri, State);
                throw new SocketCommunicatorException($"WebSocket connection failed [state: {State}]", ex);
            }
        }
        protected async Task SendCommandAsync(string message)
        {
            if (DiagnosticEnabled)
            {
                _logger.LogInformation("{class}::{method}: sending message: {message} [state: {state}]",
                    nameof(WebSocketClient), nameof(SendCommandAsync), message, State);
            }

            try
            {
                await _communicator.SendAsync(message, _cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{class}::{method}: sending message: {message} failed [state: {state}]",
                    nameof(WebSocketClient), nameof(SendCommandAsync), message, State);
                throw;
            }
        }
        protected virtual void OnMessageArrived(string message)
        {
            if (DiagnosticEnabled)
            {
                _logger.LogTrace("{class}::{method}: received message: " + message, nameof(WebSocketClient), nameof(OnMessageArrived));
            }

            ProcessMessage(message);
            MessageArrived?.Invoke(message);
        }

        protected virtual void OnSocketConnectionError(Exception error)
        {
            if (DiagnosticEnabled)
            {
                _logger.LogError(error, "{class}::{method}: socket connection error", nameof(WebSocketClient), nameof(OnSocketConnectionError));
            }

            FireConnectionClosed(error.Message);
        }

        protected virtual void OnSocketConnectionClosed(string reason)
        {
            if (DiagnosticEnabled)
            {
                _logger.LogInformation("{class}:Socket connection closed [reason: {reason}, reconnect: {reconnect}]",
                    nameof(WebSocketClient), reason, !IsDisposing);
            }

            FireConnectionClosed(reason);
        }

        protected virtual void ProcessMessage(string message)
        {
        }

        protected void FireConnectionClosed(string reason)
        {
            ConnectionClosed?.Invoke(reason);
        }

        public async Task ReconnectAsync()
        {
            if (IsDisposing)
                return;

            var attempt = 0;
            connect:

            if (DiagnosticEnabled)
            {
                _logger.LogInformation("{class}::{method}: trying to reconnect [uri: {uri}]",
                    nameof(WebSocketClient), nameof(ReconnectAsync), Uri);
            }

            try
            {
                _communicator.ResetWebSocket();
                await EnsureConnected();
            }
            catch (SocketCommunicatorException ex)
            {
                if (DiagnosticEnabled)
                {
                    _logger.LogInformation(ex, "{class}::{method}: reconnect failed [uri: {uri}, attempt #{attempt}, error: {error}]",
                        nameof(WebSocketClient), nameof(ReconnectAsync), Uri, attempt, ex.Message);
                }

                // try a few times before throw
                attempt++;
                if (attempt < 10)
                {
                    //let some time to fix the network issue before the app fails
                    Thread.Sleep(5000 + attempt * 1000);
                    goto connect;
                }

                FireConnectionClosed(ex.Message);
            }
        }

        public void Dispose()
        {
            IsDisposing = true;
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
            _communicator.Dispose();
            _communicator = null;
        }
    }

}