using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using SpeechmaticsAPI.Enumerations;
using SpeechmaticsAPI.Interfaces;

namespace SpeechmaticsAPI
{
    using System;
    using System.Net.WebSockets;
    using System.Threading;
    using System.Threading.Tasks;

    public class SmRtApi : ISmRtApi
    {
        private readonly Stream _stream;

        public Action<string> AddTranscriptCallback { get; }
        public string Model { get; }
        public int SampleRate { get; }
        public AudioFormatType AudioFormat { get; }
        public AudioFormatEncoding AudioFormatEncoding { get; }
        public AutoResetEvent MessageLoopResetEvent { get; }

        /// <summary>
        /// Cancellation token for async operations
        /// </summary>
        public CancellationToken CancelToken { get; }

        /// <summary>
        /// The websocket URL this API instance is connected to
        /// </summary>
        public Uri WsUrl { get; }

        /// <summary>
        /// Transcribe a file from a stream
        /// </summary>
        /// <param name="wsUrl">A websocket endpoint e.g. wss://192.168.1.10:9000/</param>
        /// <param name="addTranscriptCallback">A callback function for the AddTranscript message</param>
        /// <param name="model">Language model</param>
        /// <param name="stream">A stream to read input from</param>
        public SmRtApi(string wsUrl,
            Action<string> addTranscriptCallback,
            CultureInfo model,
            Stream stream) : this(wsUrl, addTranscriptCallback, model, stream, AudioFormatType.File, AudioFormatEncoding.File, 0)
        {
        }

        /// <summary>
        /// Transcribe raw audio from a stream
        /// </summary>
        /// <param name="wsUrl">A websocket endpoint e.g. wss://192.168.1.10:9000/</param>
        /// <param name="addTranscriptCallback">A callback function for the AddTranscript message</param>
        /// <param name="model">Language model</param>
        /// <param name="stream">A stream to read input from</param>
        /// <param name="audioFormat">Raw</param>
        /// <param name="audioFormatEncoding">PCM encoding type</param>
        /// <param name="sampleRate">e.g. 16000, 44100</param>
        public SmRtApi(string wsUrl,
            Action<string> addTranscriptCallback,
            CultureInfo model,
            Stream stream,
            AudioFormatType audioFormat,
            AudioFormatEncoding audioFormatEncoding,
            int sampleRate)
        {
            if (audioFormat == AudioFormatType.File && audioFormatEncoding != AudioFormatEncoding.File
                || audioFormatEncoding == AudioFormatEncoding.File && audioFormat != AudioFormatType.File)
            {
                throw new ArgumentException("audioFormat and audioFormatEncoding must both be File");
            }

            AddTranscriptCallback = addTranscriptCallback;
            AudioFormat = audioFormat;
            AudioFormatEncoding = audioFormatEncoding;
            SampleRate = sampleRate;
            Model = model.Name;
            MessageLoopResetEvent = new AutoResetEvent(false);
            WsUrl = new Uri(wsUrl);
            _stream = stream;

            var src = new CancellationTokenSource();
            CancelToken = src.Token;
        }

        /// <summary>
        /// Start the message loop and do not return until the file is transcribed
        /// </summary>
        [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
        // Justification: The AutoResetEvent prevent the using block from terminating until the web socket client is no longer needed.
        public void Run()
        {
            using (var wsClient = new ClientWebSocket())
            {
                MessageLoopResetEvent.Reset();

                var connect = wsClient.ConnectAsync(WsUrl, CancelToken);
                Debug.WriteLine("Starting connection");
                connect.Wait(CancelToken);
                if (connect.IsFaulted || wsClient.State != WebSocketState.Open)
                {
                    throw new InvalidOperationException("Connection failed");
                }
                Debug.WriteLine("Connection succeeded");

                /* The reading loop */
                Task.Factory.StartNew(async () =>
                {
                    var reader = new MessageReader(this, wsClient);
                    await reader.Start();

                }, CancelToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);

                /* The writing loop */
                Task.Factory.StartNew(async () =>
                {
                    var writer = new MessageWriter(this, wsClient, _stream);
                    await writer.Start();
                }, CancelToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);

                MessageLoopResetEvent.WaitOne();
            }
        }
    }
}