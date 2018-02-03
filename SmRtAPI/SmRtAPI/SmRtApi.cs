using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using SpeechmaticsAPI.Enumerations;

namespace SpeechmaticsAPI
{
    using System;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Messages;

    public class SmRtApi
    {
        private readonly Stream _stream;
        private readonly AutoResetEvent _resetEvent;
        private readonly ClientWebSocket _wsClient;
        private readonly CancellationToken _cancellationToken;
        private readonly string _model;
        private int _sequenceNumber;
        private int _ackedSequenceNumbers;

        public Uri WsUrl { get; }

        public SmRtApi(string wsUrl, CultureInfo model, Stream stream = null)
        {
            _stream = stream;
            _model = model.Name;
            _resetEvent = new AutoResetEvent(false);
            WsUrl = new Uri(wsUrl);
            _wsClient = new ClientWebSocket();
            var src = new CancellationTokenSource();
            _cancellationToken = src.Token;
        }

        public void Run()
        {
            _resetEvent.Reset();

            Task.Factory.StartNew(async () =>
            {
                var receiveBuffer = new ArraySegment<byte>(new byte[32768]);

                var connect = _wsClient.ConnectAsync(WsUrl, _cancellationToken);
                Debug.WriteLine("Starting connection");
                await connect;
                if (connect.IsFaulted || _wsClient.State != WebSocketState.Open)
                {
                    throw new InvalidOperationException("Connection failed");
                }

                Debug.WriteLine("Connection succeeded");
                await StartRecognition();

                var streamBuffer = new byte[2048];
                int bytesRead;

                while ((bytesRead = _stream.Read(streamBuffer, 0, streamBuffer.Length)) > 0)
                {
                    var webSocketReceiveResult = await _wsClient.ReceiveAsync(receiveBuffer, _cancellationToken);
                    if (ProcessMessage(webSocketReceiveResult, receiveBuffer))
                    {
                        break;
                    }
                    await SendData(new ArraySegment<byte>(streamBuffer, 0, bytesRead));
                }
                _resetEvent.Set();
            }, _cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            _resetEvent.WaitOne();
        }

        private bool ProcessMessage(WebSocketReceiveResult result, ArraySegment<byte> message)
        {
            var subset = new ArraySegment<byte>(message.Array, 0, result.Count);
            var messageAsString = Encoding.UTF8.GetString(subset.ToArray());
            dynamic jsonObject = JObject.Parse(messageAsString);

            switch (jsonObject.Value<string>("message"))
            {
                case "RecognitionStarted":
                {
                    Console.WriteLine("Recognition started");
                    break;
                }
                case "DataAdded":
                {
                    Console.WriteLine("Got ack for {0}", jsonObject.Value<int>("seq_no"));
                    Interlocked.Increment(ref _ackedSequenceNumbers);
                    break;
                }
                default:
                {
                    Console.WriteLine(messageAsString);
                    break;
                }
            }
            return result.MessageType == WebSocketMessageType.Close;
        }

        /// <summary>
        /// Send a block of data as a sequence of AddData and binary messages.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        async Task SendData(ArraySegment<byte> data)
        {
            // This is the size of the binary websocket message
            const int messageBlockSize = 1024;

            var arrayCopy = data.ToArray();
            var finalSectionOffset = 0;

            var msg = new AddDataMessage
            {
                size = data.Count,
                offset = 0,
                seq_no = Interlocked.Add(ref _sequenceNumber, 1)
            };

            Debug.WriteLine("seq_no = {0}, acked = {1}", msg.seq_no, _ackedSequenceNumbers);

            //while (msg.seq_no - _ackedSequenceNumbers > 20)
            //{
            //    Debug.WriteLine("seq_no = {0}, acked = {1}", msg.seq_no, _ackedSequenceNumbers);
            //    Thread.Sleep(100);
            //}

            await msg.Send(_wsClient, _cancellationToken);

            if (data.Count > messageBlockSize)
            {
                for (var offset = 0; offset < data.Count / messageBlockSize; offset += messageBlockSize)
                {
                    finalSectionOffset += messageBlockSize;
                    await _wsClient.SendAsync(new ArraySegment<byte>(arrayCopy, offset, messageBlockSize),
                        WebSocketMessageType.Binary, false, _cancellationToken);
                }
            }

            await _wsClient.SendAsync(
                new ArraySegment<byte>(arrayCopy, finalSectionOffset, data.Count - finalSectionOffset),
                WebSocketMessageType.Binary, true, _cancellationToken);
        }

        async Task StartRecognition()
        {
            var audioFormat = new AudioFormatSubMessage(AudioFormatType.File, AudioFormatEncoding.File, 44100);
            var msg = new StartRecognitionMessage(audioFormat, _model, OutputFormat.Json, "rt_test");
            await msg.Send(_wsClient, _cancellationToken);
        }
    }
}