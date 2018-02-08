using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Speechmatics.Realtime.Client.Interfaces;

namespace Speechmatics.Realtime.Client
{
    /// <summary>
    /// Speechmatics realtime API.
    /// 
    /// Each instance represents a separate connection to the transcription engine.
    /// </summary>
    public class SmRtApi : ISmRtApi
    {
        private readonly Stream _stream;

        /// <summary>
        /// Callback executed when the AddTranscript message is received from the server.
        /// </summary>
        public Action<string> AddTranscriptCallback { get; }
        /// <summary>
        /// Configuration object - audio properties and language
        /// </summary>
        public SmRtApiConfig Configuration { get; }
        /// <summary>
        /// Cancellation token for async operations
        /// </summary>
        public CancellationToken CancelToken { get; }
        /// <summary>
        /// The websocket URL this API instance is connected to
        /// </summary>
        public Uri WsUrl { get; }

        /// <summary>
        /// Transcribe raw audio from a stream
        /// </summary>
        /// <param name="wsUrl">A websocket endpoint e.g. wss://192.168.1.10:9000/</param>
        /// <param name="addTranscriptCallback">A callback function for the AddTranscript message</param>
        /// <param name="stream">A stream to read input from</param>
        /// <param name="configuration">Configuration object (model, audio properties)</param>
        public SmRtApi(string wsUrl,
            Action<string> addTranscriptCallback,
            Stream stream,
            SmRtApiConfig configuration)
        {
            AddTranscriptCallback = addTranscriptCallback;
            Configuration = configuration;
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