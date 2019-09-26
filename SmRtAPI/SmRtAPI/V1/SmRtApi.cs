using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Speechmatics.Realtime.Client.V1.Config;
using Speechmatics.Realtime.Client.V1.Interfaces;

namespace Speechmatics.Realtime.Client.V1
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
        /// <param name="stream">A stream to read input from</param>
        /// <param name="configuration">Configuration object (model, audio properties)</param>
        public SmRtApi(string wsUrl,
            Stream stream,
            SmRtApiConfig configuration)
        {
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
            using (var recognitionStarted = new AutoResetEvent(false))
            {
                using (var transcriptionComplete = new AutoResetEvent(false))
                {
                    using (var wsClient = new ClientWebSocket())
                    {
                        if (Configuration.Insecure)
                        {
                            // TODO: Support this, but .NET Standard doesn't implement insecure websockets yet
                            // https://github.com/dotnet/corefx/issues/5120
                            // It's done in .NET Core 2.1, but .NET Standard 2.1 doesn't exist yet
                            ServicePointManager.ServerCertificateValidationCallback =
                                (sender, certificate, chain, errors) => true;
                        }

                        var connect = wsClient.ConnectAsync(WsUrl, CancelToken);
                        Trace.WriteLine("Starting connection");
                        connect.Wait(CancelToken);
                        if (connect.IsFaulted || wsClient.State != WebSocketState.Open)
                        {
                            throw new InvalidOperationException("Connection failed");
                        }
                        Trace.WriteLine("Connection succeeded");

                        /* The reading loop */
                        var t1 = Task.Factory.StartNew(async () =>
                        {
                            var reader = new MessageReader(this, wsClient, transcriptionComplete, recognitionStarted);
                            await reader.Start();

                        }, CancelToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);

                        /* The writing loop */
                        var t2 = Task.Factory.StartNew(async () =>
                        {
                            var writer = new MessageWriter(this, wsClient, transcriptionComplete, _stream, recognitionStarted);
                            await writer.Start();
                        }, CancelToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);

                        transcriptionComplete.WaitOne();
                        Task.WaitAll(t1, t2);
                    }
                }
            }
        }
    }
}