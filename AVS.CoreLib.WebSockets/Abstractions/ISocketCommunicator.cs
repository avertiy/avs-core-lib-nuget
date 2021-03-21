using System;
using System.Threading.Tasks;

namespace AVS.CoreLib.WebSockets
{
    /// <summary>
    /// Wrapper for low level communication details for ClientWebSocket
    /// </summary>
    public interface ISocketCommunicator: IDisposable
    {
        bool IsConnected { get; }
        /// <summary>
        /// Sends command data on <see cref="T:System.Net.WebSockets.ClientWebSocket" /> as an asynchronous operation.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task SendAsync(IChannelCommand command);
        
        Task ReconnectAsync();

        event Action<string> MessageArrived;

        event Action ConnectionClosed;

        event Action<Exception> ConnectionError;
    }
}