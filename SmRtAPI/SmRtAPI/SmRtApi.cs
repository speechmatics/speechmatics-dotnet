﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Speechmatics.Realtime.Client.Config;
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

        private MessageWriter? _writer;

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
        /// Initializes a new instance of the <see cref="SmRtApi"/> class with the specified parameters.
        /// </summary>
        /// <param name="wsUrl">The websocket endpoint URL.</param>
        /// <param name="stream">The input stream for audio data.</param>
        /// <param name="configuration">The configuration object containing model and audio properties.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        public SmRtApi(string wsUrl,
            Stream stream,
            SmRtApiConfig configuration,
            CancellationToken cancellationToken)
        {
            Configuration = configuration;
            WsUrl = new Uri(wsUrl);
            _stream = stream;

            CancelToken = cancellationToken;
        }

        /// <summary>
        /// Starts the transcription process synchronously and waits for it to complete.
        /// </summary>
        public void Run()
        {
            Task.WaitAll(RunAsync());
        }

        private async Task<string> GenerateTempToken(string? authToken)
        {
            if (string.IsNullOrEmpty(authToken))
            {
                throw new InvalidOperationException("Auth token is required to generate a temporary token");
            }
            using (HttpClient httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://mp.speechmatics.com/v1/api_keys?type=rt");

                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);
                request.Content = new StringContent("{\"ttl\": 3600 }", System.Text.Encoding.UTF8, "application/json");
                var response = await httpClient.SendAsync(request).ConfigureAwait(false);
                var json = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                if (values != null)
                {
                    return values["key_value"];
                }
                throw new InvalidOperationException($"Failed to generate temporary token from {json}");
            }
        }

        /// <summary>
        /// Start the message loop and do not return until the file is transcribed
        /// </summary>
        [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
        // Justification: The AutoResetEvent prevent the using block from terminating until the web socket client is no longer needed.
        public async Task RunAsync()
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
                        if (Configuration.GenerateTempToken)
                        {
                            var tempToken = await GenerateTempToken(Configuration.AuthToken);
                            wsClient.Options.SetRequestHeader("Authorization", $"Bearer {tempToken}");
                        }
                        else
                        {
                            wsClient.Options.SetRequestHeader("Authorization", $"Bearer {Configuration.AuthToken}");
                        }

                        await wsClient.ConnectAsync(WsUrl, CancelToken);
                        Trace.WriteLine("Starting connection");
                        if (wsClient.State != WebSocketState.Open)
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
                            _writer = new MessageWriter(this, wsClient, transcriptionComplete, _stream, recognitionStarted);
                            await _writer.Start();
                        }, CancelToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);

                        transcriptionComplete.WaitOne();
                        await Task.WhenAll(t1, t2);
                    }
                }
            }
        }

        /// <summary>
        /// Change recognition config while transcription is in progress
        /// </summary>
        /// <param name="maxDelay"></param>
        /// <param name="maxDelayMode"></param>
        /// <param name="enablePartials"></param>
        public async void SetRecognitionConfig(double? maxDelay = null, string? maxDelayMode = null, bool? enablePartials = null)
        {
            if (maxDelay.HasValue)
            {
                Configuration.MaxDelay = maxDelay.Value;
            }
            if (maxDelayMode != null)
            {
                Configuration.MaxDelayMode = maxDelayMode;
            }
            if (enablePartials.HasValue)
            {
                Configuration.EnablePartials = enablePartials.Value;
            }
            if (_writer != null)
            {
                await _writer.SetRecognitionConfig();
            }
        }
    }
}
