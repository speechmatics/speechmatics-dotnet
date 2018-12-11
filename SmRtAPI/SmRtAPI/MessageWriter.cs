using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Speechmatics.Realtime.Client.Enumerations;
using Speechmatics.Realtime.Client.Interfaces;
using Speechmatics.Realtime.Client.Messages;

namespace Speechmatics.Realtime.Client
{
    internal class MessageWriter
    {
        private readonly ClientWebSocket _wsClient;
        private readonly AutoResetEvent _transcriptionComplete;
        private int _sequenceNumber;
        private readonly Stream _stream;
        private readonly AutoResetEvent _recognitionStarted;
        private readonly ISmRtApi _api;

        internal MessageWriter(ISmRtApi smRtApi,
            ClientWebSocket client,
            AutoResetEvent transcriptionComplete,
            Stream stream,
            AutoResetEvent recognitionStarted)
        {
            _api = smRtApi;
            _stream = stream;
            _recognitionStarted = recognitionStarted;
            _wsClient = client;
            _transcriptionComplete = transcriptionComplete;
        }

        public async Task Start()
        {
            await StartRecognition();

            // TODO: make limit configurable
            if (!_recognitionStarted.WaitOne(10000))
            {
                Trace.Write("Recognition started not received");
                _transcriptionComplete.Set();
                throw new InvalidOperationException("Recognition started not received");
            }

            if (_api.Configuration.CustomDictionaryPlainWords != null ||
                _api.Configuration.CustomDictionarySoundsLikes != null ||
                _api.Configuration.OutputLocale != null ||
                _api.Configuration.DynamicTranscriptConfiguration != null)
            {
                await SetRecognitionConfig();
            }

            var streamBuffer = new byte[2048];
            int bytesRead;

            while ((bytesRead = _stream.Read(streamBuffer, 0, streamBuffer.Length)) > 0 && !_transcriptionComplete.WaitOne(0))
            {
                await SendData(new ArraySegment<byte>(streamBuffer, 0, bytesRead));
            }

            var endOfStream = new EndOfStreamMessage(_sequenceNumber);
            await endOfStream.Send(_wsClient, _api.CancelToken);
        }

        /// <summary>
        /// Send a block of data as a sequence of AddData and binary messages.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private async Task SendData(ArraySegment<byte> data)
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

            await msg.Send(_wsClient, _api.CancelToken);

            if (data.Count > messageBlockSize)
            {
                for (var offset = 0; offset < data.Count / messageBlockSize; offset += messageBlockSize)
                {
                    finalSectionOffset += messageBlockSize;
                    await _wsClient.SendAsync(new ArraySegment<byte>(arrayCopy, offset, messageBlockSize),
                        WebSocketMessageType.Binary, false, _api.CancelToken);
                }
            }

            await _wsClient.SendAsync(
                new ArraySegment<byte>(arrayCopy, finalSectionOffset, data.Count - finalSectionOffset),
                WebSocketMessageType.Binary, true, _api.CancelToken);
        }

        private async Task StartRecognition()
        {
            var audioFormat = new AudioFormatSubMessage(_api.Configuration.AudioFormat,
                _api.Configuration.AudioFormatEncoding,
                _api.Configuration.SampleRate);
            var msg = new StartRecognitionMessage(audioFormat, _api.Configuration.Model, OutputFormat.Json, "rt_test");
            await msg.Send(_wsClient, _api.CancelToken);
        }

        private async Task SetRecognitionConfig()
        {
            var additionalVocab = new AdditionalVocabSubMessage(_api.Configuration.CustomDictionaryPlainWords, _api.Configuration.CustomDictionarySoundsLikes);
            OutputLocaleSubMessage outputLocale = null;
            DynamicTranscriptSubMessage dynamicTranscriptSubMessage = null;

            if (!string.IsNullOrEmpty(_api.Configuration.OutputLocale))
            {
                outputLocale = new OutputLocaleSubMessage(_api.Configuration.OutputLocale);
            }

            if (_api.Configuration.DynamicTranscriptConfiguration != null)
            {
                dynamicTranscriptSubMessage = new DynamicTranscriptSubMessage(_api.Configuration.DynamicTranscriptConfiguration);
            }

            var msg = new SetRecognitionConfigMessage(additionalVocab, outputLocale, dynamicTranscriptSubMessage);
            await msg.Send(_wsClient, _api.CancelToken);
        }
    }
}