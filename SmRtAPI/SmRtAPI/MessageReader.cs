using System;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Speechmatics.Realtime.Client.Messages;
using Speechmatics.Realtime.Client.Interfaces;

namespace Speechmatics.Realtime.Client
{
    internal class MessageReader
    {
        private string _lastPartial;
        private int _ackedSequenceNumbers;
        private readonly ClientWebSocket _wsClient;
        private readonly AutoResetEvent _resetEvent;
        private readonly AutoResetEvent _recognitionStarted;
        private readonly ISmRtApi _api;
        private readonly StringBuilder _multipartMessageBuffer;

        internal MessageReader(ISmRtApi smRtApi, ClientWebSocket client, AutoResetEvent resetEvent, AutoResetEvent recognitionStarted)
        {
            _api = smRtApi;
            _wsClient = client;
            _resetEvent = resetEvent;
            _recognitionStarted = recognitionStarted;
            _multipartMessageBuffer = new StringBuilder();
            _lastPartial = string.Empty;
        }

        internal async Task Start()
        {
            var receiveBuffer = new ArraySegment<byte>(new byte[32768]);

            while (true)
            {
                var webSocketReceiveResult = await _wsClient.ReceiveAsync(receiveBuffer, _api.CancelToken) ?? throw new InvalidOperationException("_wsClient.ReceiveAsync: webSocketReceiveResult is null");
                if (ProcessMessage(webSocketReceiveResult, receiveBuffer))
                {
                    _resetEvent.Set();
                    break;
                }
            }
        }

        private bool ProcessMessage(WebSocketReceiveResult result, ArraySegment<byte> message)
        {
            Debug.Assert(message.Array != null);
            // Return true if the message should cause the loop to exit, false otherwise.
            var subset = new ArraySegment<byte>(message.Array, 0, result.Count);
            var subMessageAsString = Encoding.UTF8.GetString(subset.ToArray());

            _multipartMessageBuffer.Append(subMessageAsString);

            if (!result.EndOfMessage)
            {
                // Seems like this will never happen (ie a Close message also being an incomplete message...)
                return result.MessageType == WebSocketMessageType.Close;
            }

            var messageAsString = _multipartMessageBuffer.ToString();
            var jsonObject = JObject.Parse(_multipartMessageBuffer.ToString());

            Trace.WriteLine("ProcessMessage: " + _multipartMessageBuffer);

            switch (jsonObject.Value<string>("message"))
            {
                case "RecognitionStarted":
                    {
                        Trace.WriteLine("Recognition started");
                        _recognitionStarted.Set();
                        break;
                    }
                case "AudioAdded":
                    {
                        // Log the ack
                        Interlocked.Increment(ref _ackedSequenceNumbers);
                        break;
                    }
                case "AddTranscript":
                    {
                        string transcript = jsonObject["metadata"]?["transcript"]?.Value<string>() ?? string.Empty;
                        var obj = JsonConvert.DeserializeObject<AddTranscriptMessage>(messageAsString) ?? throw new InvalidOperationException("AddTranscriptMessage is null");
                        _api.Configuration.AddTranscriptMessageCallback?.Invoke(obj);
                        _api.Configuration.AddTranscriptCallback?.Invoke(transcript);
                        break;
                    }
                case "AddPartialTranscript":
                    {
                        _lastPartial = jsonObject["metadata"]?["transcript"]?.Value<string>() ?? string.Empty;
                        var obj = JsonConvert.DeserializeObject<AddPartialTranscriptMessage>(messageAsString) ?? throw new InvalidOperationException("AddPartialTranscriptMessage is null");
                        _api.Configuration.AddPartialTranscriptMessageCallback?.Invoke(obj);
                        _api.Configuration.AddPartialTranscriptCallback?.Invoke(_lastPartial);
                        break;
                    }
                case "AddTranslation":
                    {
                        var obj = JsonConvert.DeserializeObject<AddTranslationMessage>(messageAsString) ?? throw new InvalidOperationException("AddTranslationMessage is null");
                        _api.Configuration.AddTranslationMessageCallback?.Invoke(obj);
                        break;
                    }
                case "AddPartialTranslation":
                    {
                        var obj = JsonConvert.DeserializeObject<AddPartialTranslationMessage>(messageAsString) ?? throw new InvalidOperationException("AddPartialTranslationMessage is null");
                        _api.Configuration.AddPartialTranslationMessageCallback?.Invoke(obj);
                        break;
                    }
                case "EndOfTranscript":
                    {
                        // Sometimes there is a partial without a corresponding transcript, let's pretend it was a transcript here.
                        _api.Configuration.AddTranscriptCallback?.Invoke(_lastPartial);
                        _api.Configuration.EndOfTranscriptCallback?.Invoke();
                        return true;
                    }
                case "Error":
                    {
                        var obj = JsonConvert.DeserializeObject<ErrorMessage>(messageAsString) ?? throw new InvalidOperationException("ErrorMessage is null");
                        _api.Configuration.ErrorMessageCallback?.Invoke(obj);
                        return true;
                    }
                case "Warning":
                    {
                        var obj = JsonConvert.DeserializeObject<WarningMessage>(messageAsString) ?? throw new InvalidOperationException("WarningMessage is null");
                        _api.Configuration.WarningMessageCallback?.Invoke(obj);
                        break;
                    }
                default:
                    {
                        Trace.WriteLine(messageAsString);
                        break;
                    }
            }

            _multipartMessageBuffer.Clear();
            return result.MessageType == WebSocketMessageType.Close;
        }
    }
}