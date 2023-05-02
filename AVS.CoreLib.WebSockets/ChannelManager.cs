using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using AVS.CoreLib.WebSockets.Abstractions;
using Microsoft.Extensions.Logging;

namespace AVS.CoreLib.WebSockets
{
    /// <summary>
    /// An abstraction layer on websocket client, help to deal with a certain websocket api url, subscribing on channel(s)/stream(s)
    /// </summary>
    public class ChannelManager : IChannelManager
    {
        private readonly ILogger _logger;
        private readonly IMessageProcessor _messageProcessor;
        private readonly Dictionary<string, string> _channels = new Dictionary<string, string>();

        #region props

        public bool AutoReconnect { get; set; }

        public int Count => _channels.Count;

        public string BaseAddress { get; set; }

        public IWebSocketClient Client { get; } 
        #endregion

        public event Action<string> ConnectionClosed;

        public ChannelManager(ILogger logger, IMessageProcessor messageProcessor) : this(logger, messageProcessor, new WebSocketClient(logger))
        {
        }

        public ChannelManager(ILogger logger, IMessageProcessor messageProcessor, IWebSocketClient client)
        {
            _logger = logger;
            _messageProcessor = messageProcessor;
            Client = client;
            Client.ConnectionClosed += OnConnectionClosed;
            Client.MessageArrived += OnMessageArrived;
        }

        protected void OnMessageArrived(string message)
        {
            _messageProcessor.ProcessMessage(message);
        }

        private async void OnConnectionClosed(string reason)
        {
            if (AutoReconnect && _channels.Any())
            {
                await Client.ReconnectAsync().ConfigureAwait(false);

                if (Client.State == WebSocketState.Open)
                {
                    // re-subscribe on channels
                    foreach (var kp in _channels)
                    {
                        await Client.SendAsync(BaseAddress, kp.Value);
                    }

                    return;
                }
            }

            _logger.LogInformation("{class}: websocket connection `{address}` has been closed [{error}]",
                nameof(ChannelManager), BaseAddress, reason);

            ConnectionClosed?.Invoke(reason);
        }

        public async Task Subscribe(string channelName, string command, bool autoReconnect = false)
        {
            await Client.SendAsync(BaseAddress, command);
            
            if(autoReconnect)
                _channels.Add(channelName, command);
        }

        public async Task UnSubscribe(string channelName, string command)
        {
            await Client.SendAsync(BaseAddress, command);

            // remove channel from auto-reconnect list
            if (_channels.ContainsKey(channelName))
            {
                _channels.Remove(channelName);
            }
        }
        
        public void Dispose()
        {
            Client?.Dispose();
        }

        public void ClearChannels()
        {
            _channels.Clear();
        }
    }
}