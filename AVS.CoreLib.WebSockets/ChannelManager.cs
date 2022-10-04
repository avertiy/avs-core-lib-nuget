using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using AVS.CoreLib.WebSockets.Abstractions;
using Microsoft.Extensions.Logging;

namespace AVS.CoreLib.WebSockets
{
    public class ChannelManager : IChannelManager
    {
        private readonly ILogger _logger;
        private readonly IMessageProcessor _messageProcessor;
        private readonly Dictionary<string, string> _channels = new Dictionary<string, string>();

        public bool AutoReconnect => _channels.Any();

        public bool DiagnosticEnabled
        {
            get => WebSocketClient.DiagnosticEnabled; 
            set => WebSocketClient.DiagnosticEnabled = value;
        }

        public TimeSpan KeepAliveInterval
        {
            get => WebSocketClient.KeepAliveInterval;
            set => WebSocketClient.KeepAliveInterval = value;
        }

        public Uri Uri
        {
            get => WebSocketClient.Uri;
            set => WebSocketClient.Uri = value;
        }

        public WebSocketClient WebSocketClient { get; }

        public event Action<string> MessageArrived;
        public event Action<Exception> ConnectionClosed;

        public ChannelManager(ILogger logger, IMessageProcessor messageProcessor) : this(logger, messageProcessor, new WebSocketClient(logger))
        {
        }

        public ChannelManager(ILogger logger, IMessageProcessor messageProcessor, string wssUrl) 
            : this(logger, messageProcessor, new WebSocketClient(logger) { Uri = new Uri(wssUrl) })
        {
        }

        public ChannelManager(ILogger logger, IMessageProcessor messageProcessor, WebSocketClient client)
        {
            _logger = logger;
            _messageProcessor = messageProcessor;
            WebSocketClient = client;
            WebSocketClient.ConnectionClosed += OnConnectionClosed;
            WebSocketClient.MessageArrived += OnMessageArrived;
        }

        protected void OnMessageArrived(string message)
        {
            _messageProcessor.ProcessMessage(message);
            MessageArrived?.Invoke(message);
        }

        private async void OnConnectionClosed(Exception error)
        {
            if (AutoReconnect)
            {
                await WebSocketClient.ReconnectAsync().ConfigureAwait(false);
                
                if (WebSocketClient.State == WebSocketState.Open)
                {
                    _logger.LogError("{class}::{method}: reconnect was not successful",
                        nameof(ChannelManager), nameof(OnConnectionClosed));
                }
                else
                {
                    // re-subscribe on channels
                    foreach (var kp in _channels)
                    {
                        await WebSocketClient.SendAsync(kp.Value);
                    }
                    return;
                }
            }

            ConnectionClosed?.Invoke(error);
        }

        public async Task Subscribe(string channel, string command, bool autoReconnect = false)
        {
            await WebSocketClient.SendAsync(command);
            
            if(autoReconnect)
                _channels.Add(channel, command);
        }

        public async Task UnSubscribe(string channel, string command)
        {
            await WebSocketClient.SendAsync(command);

            if (_channels.ContainsKey(channel))
                _channels.Remove(channel);
        }
        
        public void Dispose()
        {
            WebSocketClient?.Dispose();
        }

        public void ClearChannels()
        {
            _channels.Clear();
        }
    }
}