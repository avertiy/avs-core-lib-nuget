using System;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace AVS.CoreLib.WebSockets.Extensions
{
    internal static class ClientWebSocketExtensions
    {
        public static async Task<WebSocketReceiveResult> GetWebSocketReceiveResultAsync(this ClientWebSocket webSocket,
            MemoryStream memoryStream, ArraySegment<byte> receivedDataBuffer, CancellationToken cancellationToken)
        {
            try
            {
                WebSocketReceiveResult webSocketReceiveResult;
                do
                {
                    webSocketReceiveResult =
                        await webSocket.ReceiveAsync(receivedDataBuffer, cancellationToken)
                            .ConfigureAwait(false);

                    await memoryStream.WriteAsync(receivedDataBuffer.Array,
                            receivedDataBuffer.Offset,
                            webSocketReceiveResult.Count,
                            cancellationToken)
                        .ConfigureAwait(false);
                } while (!webSocketReceiveResult.EndOfMessage);


                return webSocketReceiveResult;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}