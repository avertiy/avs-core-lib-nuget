using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace AVS.CoreLib.WebSockets.Abstractions
{
    public interface IWebSocketClient : IDisposable
    {
        bool IsConnected { get; }
        WebSocketState State { get; }
        ClientWebSocketOptions Options { get; }
        bool DiagnosticEnabled { get; set; }
        TimeSpan KeepAliveInterval { get; set; }

        Task SendAsync(string url, string command);
        Task ReconnectAsync();

        event Action<string> ConnectionClosed;
        event Action<string> MessageArrived;
    }
}