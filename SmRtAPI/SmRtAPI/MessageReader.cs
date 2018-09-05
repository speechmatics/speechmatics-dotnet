using System;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Speechmatics.Realtime.Client.Interfaces;
using Speechmatics.Realtime.Client.Messages;

namespace Speechmatics.Realtime.Client
{
    internal class MessageReader
    {
        private int _ackedSequenceNumbers;
        private readonly ClientWebSocket _wsClient;
        private readonly AutoResetEvent _resetEvent;
        private readonly Flag _recognitionStarted;
        private readonly ISmRtApi _api;

        internal MessageReader(ISmRtApi smRtApi, ClientWebSocket client, AutoResetEvent resetEvent, Flag recognitionStarted)
        {
            _api = smRtApi;
            _wsClient = client;
            _resetEvent = resetEvent;
            _recognitionStarted = recognitionStarted;
        }

        internal async Task Start()
        {
            var receiveBuffer = new ArraySegment<byte>(new byte[32768]);

            while (true)
            {
                var webSocketReceiveResult = await _wsClient.ReceiveAsync(receiveBuffer, _api.CancelToken);
                if (ProcessMessage(webSocketReceiveResult, receiveBuffer))
                {
                    _resetEvent.Set();
                    break;
                }
            }
        }

        private bool ProcessMessage(WebSocketReceiveResult result, ArraySegment<byte> message)
        {
            var subset = new ArraySegment<byte>(message.Array, 0, result.Count);
            var messageAsString = Encoding.UTF8.GetString(subset.ToArray());
            var jsonObject = JObject.Parse(messageAsString);

            switch (jsonObject.Value<string>("message"))
            {
                case "RecognitionStarted":
                {
                    Debug.WriteLine("Recognition started");
                    _recognitionStarted.Status = true;
                    break;
                }
                case "DataAdded":
                {
                    // Log the ack
                    Interlocked.Increment(ref _ackedSequenceNumbers);
                    break;
                }
                case "AddTranscript":
                {
                    string transcript = jsonObject.Value<string>("transcript");
                    _api.Configuration.AddTranscriptCallback?.Invoke(transcript);
                    _api.Configuration.AddTranscriptMessageCallback?.Invoke(JsonConvert.DeserializeObject<AddTranscriptMessage>(messageAsString));
                    break;
                }
                case "AddPartialTranscript":
                {
                    _api.Configuration.AddPartialTranscriptMessageCallback?.Invoke(JsonConvert.DeserializeObject<AddTranscriptMessage>(messageAsString));
                    break;
                }
                case "EndOfTranscript":
                {
                    _api.Configuration.EndOfTranscriptCallback?.Invoke();
                    return true;
                }
                case "Error":
                {
                    _api.Configuration.ErrorMessageCallback?.Invoke(JsonConvert.DeserializeObject<ErrorMessage>(messageAsString));
                    return true;
                }
                case "Warning":
                {
                    _api.Configuration.WarningMessageCallback?.Invoke(JsonConvert.DeserializeObject<WarningMessage>(messageAsString));
                    break;
                }
                default:
                {
                    Debug.WriteLine(messageAsString);
                    break;
                }
            }
            return result.MessageType == WebSocketMessageType.Close;
        }
    }
}