using System;
using System.Diagnostics;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SpeechmaticsAPI.Messages
{
    public abstract class BaseMessage
    {
        public string AsJson()
        {
            using (var sw = new StringWriter())
            {
                JsonSerializer.Create().Serialize(sw, this);
                return sw.ToString();
            }
        }

        public async Task Send(ClientWebSocket webSocket, CancellationToken token)
        {
            var asJson = AsJson();

            var bytes = Encoding.UTF8.GetBytes(asJson);
            await webSocket.SendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length), WebSocketMessageType.Text, true,
                token).ContinueWith(t =>
            {
                Debug.WriteLine("Sent {0} {1}, faulted={2}, status={3}", message, asJson, t.IsFaulted, t.Status);
            }, token);
        }

        public abstract string message { get; }
    }
}