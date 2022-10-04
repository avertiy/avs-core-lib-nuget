using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using AVS.CoreLib.WebSockets.Abstractions;
using Microsoft.Extensions.Logging;

namespace AVS.CoreLib.WebSockets
{
    /// <summary>
    /// Represents base web socket client that utilize <seealso cref="ISocketCommunicator"/> to subscribe on wss channel
    /// with auto reconnect option
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

        public TimeSpan KeepAliveInterval
        {
            get => _communicator.KeepAliveInterval;
            set => _communicator.KeepAliveInterval = value;
        }

        public ClientWebSocketOptions Options => _communicator.Options;


        public Uri Uri
        {
            get => _uri == null ? throw new InvalidOperationException("Uri was not initialized") : _uri;
            set => _uri = value;
        }

        #endregion

        public event Action<Exception> ConnectionClosed;

        /// <summary>
        /// use MessageArrived event if WebSocketClient is used directly, if it is inherited override ProcessMessage
        /// </summary>
        public event Action<string> MessageArrived;

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

        public async Task SendAsync(string command)
        {
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

            FireConnectionClosed(error);
        }

        protected virtual void OnSocketConnectionClosed()
        {
            if (DiagnosticEnabled)
            {
                _logger.LogInformation("{class}::{method}: socket connection closed [reconnect: {reconnect}]",
                    nameof(WebSocketClient), nameof(OnSocketConnectionClosed), !IsDisposing);
            }

            FireConnectionClosed();
        }

        protected virtual void ProcessMessage(string message)
        {
        }

        protected void FireConnectionClosed(Exception error = null)
        {
            ConnectionClosed?.Invoke(error);
        }

        public async Task ReconnectAsync()
        {
            if (IsDisposing)
                return;

            var attempt = 0;
            connect:
            if (DiagnosticEnabled)
            {
                _logger.LogInformation("{class}::{method}: trying to reconnect",
                    nameof(WebSocketClient), nameof(ReconnectAsync));
            }

            try
            {
                _communicator.ResetWebSocket();
                await EnsureConnected();
            }
            catch (SocketCommunicatorException ex)
            {
                // try a few times before throw
                attempt++;
                if (attempt < 10)
                {
                    //let some time to fix the network issue before the app fails
                    Thread.Sleep(5000 + attempt * 1000);
                    goto connect;
                }

                FireConnectionClosed();
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

        //private async Task TestWebSocket(string commandMessage)
        //{
        //    _logger.LogInformation("{class}::{method}: Test websocket",
        //        nameof(WebSocketClient), nameof(TestWebSocket), _uri, State);
        //    await _communicator.Test(_uri, commandMessage, _cancellationToken);
        //    _logger.LogInformation("{class}::{method}: Test completed",
        //        nameof(WebSocketClient), nameof(TestWebSocket), _uri, State);
        //}
    }

}