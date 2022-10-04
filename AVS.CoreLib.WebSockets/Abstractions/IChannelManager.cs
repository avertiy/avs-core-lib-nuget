using System;
using System.Threading.Tasks;

namespace AVS.CoreLib.WebSockets.Abstractions
{
    public interface IChannelManager : IDisposable
    {
        bool DiagnosticEnabled { get; set; }
        TimeSpan KeepAliveInterval { get; set; }
        Uri Uri { get; set; }
        WebSocketClient WebSocketClient { get; }

        event Action<string> MessageArrived;
        event Action<Exception> ConnectionClosed;

        Task Subscribe(string channel, string command, bool autoReconnect = false);

        Task UnSubscribe(string channel, string command);

        void ClearChannels();
    }

    public interface IMessageProcessor
    {
        void ProcessMessage(string message);
    }
}