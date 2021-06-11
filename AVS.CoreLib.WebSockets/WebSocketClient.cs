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
    public class WebSocketClient : IDisposable
    {
        private readonly ILogger _logger;
        private ISocketCommunicator _communicator;
        private bool _disposing;
        private readonly Uri _uri;
        private CancellationTokenSource _cancellationTokenSource;
        private readonly CancellationToken _cancellationToken;
        private string _lastCommand;
        public bool IsConnected => State == WebSocketState.Open;
        public bool DiagnosticEnabled { get; set; }

        protected WebSocketState State => _communicator.State;

        protected WebSocketClient(string wssApiUrl, ILogger logger) : this(CreateCommunicator(), logger)
        {
            _uri = new Uri(wssApiUrl);
        }

        protected WebSocketClient(ISocketCommunicator communicator, ILogger logger)
        {
            _communicator = communicator;
            _logger = logger;
            _communicator.ConnectionClosed += OnSocketConnectionClosed;
            _communicator.ConnectionError += OnSocketConnectionError;
            _communicator.MessageArrived += OnMessageArrived;
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
        }

        protected async Task SendAsync(string command, bool autoReconnect)
        {
            await EnsureConnected().ConfigureAwait(false);
            await SendCommandAsync(command).ConfigureAwait(false);
            _lastCommand = autoReconnect ? command : null;
        }

        private async Task<bool> EnsureConnected()
        {
            if (State == WebSocketState.Open)
                return true;

            if(DiagnosticEnabled)
                _logger.LogInformation("{class}::{method}: web socket connecting to {uri} [state: {state}]", nameof(WebSocketClient), nameof(EnsureConnected), _uri, State);

            try
            {
                return await _communicator.ConnectAsync(_uri, _cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{class}::{method}: connect to {url} failed [state: {state}]",
                    nameof(WebSocketClient), nameof(EnsureConnected), _uri, State);
                throw new SocketCommunicatorException($"WebSocket connection failed [state: {State}]", ex);
            }
        }

        //private async Task TestWebSocket(string commandMessage)
        //{
        //    _logger.LogInformation("{class}::{method}: Test websocket",
        //        nameof(WebSocketClient), nameof(TestWebSocket), _uri, State);
        //    await _communicator.Test(_uri, commandMessage, _cancellationToken);
        //    _logger.LogInformation("{class}::{method}: Test completed",
        //        nameof(WebSocketClient), nameof(TestWebSocket), _uri, State);
        //}

        private async Task SendCommandAsync(string message)
        {
            if (DiagnosticEnabled)
            {
                _logger.LogInformation("{class}::{method}: sending message: {message} [state: {state}]",
                    nameof(WebSocketClient), nameof(SendAsync), message, State);
            }

            try
            {
                await _communicator.SendAsync(message, _cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{class}::{method}: sending message: {message} failed [state: {state}]",
                    nameof(WebSocketClient), nameof(SendAsync), message, State);
                throw;
            }
        }

        protected virtual void OnMessageArrived(string message)
        {
            if (DiagnosticEnabled)
            {
                _logger.LogTrace("message: " +message);
            }
        }

        protected virtual void OnSocketConnectionClosed()
        {
            if (DiagnosticEnabled)
            {
                _logger.LogInformation("{class}::{method}: socket connection closed [reconnect: {reconnect}]",
                    nameof(WebSocketClient), nameof(OnSocketConnectionClosed), !_disposing);
            }
        }

        protected virtual async void OnSocketConnectionError(Exception error)
        {
            if (DiagnosticEnabled)
            {
                _logger.LogError(error, "{class}::{method}: socket connection error [reconnect: {reconnect}]", 
                    nameof(WebSocketClient), nameof(OnSocketConnectionError), _lastCommand != null);
            }

            if (_lastCommand != null)
            {
                await ReconnectAsync().ConfigureAwait(false);
            }
        }

        protected virtual async Task ReconnectAsync()
        {
            if (_lastCommand == null || _disposing)
                return;

            if (DiagnosticEnabled)
            {
                _logger.LogInformation("{class}::{method}: reconnecting",
                    nameof(WebSocketClient), nameof(ReconnectAsync), _lastCommand != null);
            }

            _communicator.ResetWebSocket();
            await EnsureConnected();
            await SendCommandAsync(_lastCommand).ConfigureAwait(false);
        }
        

        public void Dispose()
        {
            _disposing = true;
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
            _communicator.Dispose();
            _communicator = null;
        }

        private static SocketCommunicator CreateCommunicator()
        {
            return new SocketCommunicator();
        }
    }
}