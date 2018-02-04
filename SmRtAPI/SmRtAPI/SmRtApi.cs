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

        /// <summary>
        /// Callback executed when the AddTranscript message is received from the server.
        /// </summary>
        public Action<string> AddTranscriptCallback { get; }
        /// <summary>
        /// Transcription language as an ISO code, e.g. en-US, en-GB, fr, ru, ja...
        /// </summary>
        public string Model { get; }
        /// <summary>
        /// Audio sample rate, e.g. 16000 (for 16kHz), 44100 (for 44.1kHz CD quality)
        /// </summary>
        public int SampleRate { get; }
        /// <summary>
        /// Enum of File or Raw
        /// </summary>
        public AudioFormatType AudioFormat { get; }
        /// <summary>
        /// If <paramref name="AudioFormat"/> is File, this must also be File. Otherwise, a choice of PCM encodings.
        /// </summary>
        public AudioFormatEncoding AudioFormatEncoding { get; }
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
            using (var resetEvent = new AutoResetEvent(false))
            {
                using (var wsClient = new ClientWebSocket())
                {
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
                        var reader = new MessageReader(this, wsClient, resetEvent);
                        await reader.Start();

                    }, CancelToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);

                    /* The writing loop */
                    Task.Factory.StartNew(async () =>
                    {
                        var writer = new MessageWriter(this, wsClient, resetEvent, _stream);
                        await writer.Start();
                    }, CancelToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);

                    resetEvent.WaitOne();
                }
            }
        }
    }
}