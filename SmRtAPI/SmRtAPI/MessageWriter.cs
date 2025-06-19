using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Speechmatics.Realtime.Client.Messages;
using Speechmatics.Realtime.Client.Interfaces;

namespace Speechmatics.Realtime.Client
{
    internal class MessageWriter
    {
        private readonly ClientWebSocket _wsClient;
        private readonly AutoResetEvent _transcriptionComplete;
        private int _sequenceNumber = 0;
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

            var streamBuffer = new byte[_api.Configuration.BlockSize];
            int bytesRead;

            while ((bytesRead = await _stream.ReadAsync(streamBuffer, 0, streamBuffer.Length, _api.CancelToken)) > 0 && !_transcriptionComplete.WaitOne(0))
            {
                await SendData(new ArraySegment<byte>(streamBuffer, 0, bytesRead));
                _sequenceNumber++;
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

            AdditionalVocabSubMessage? additionalVocab = null;
            if (_api.Configuration.CustomDictionaryPlainWords.Any() || _api.Configuration.CustomDictionarySoundsLikes.Any())
            {
                additionalVocab = new AdditionalVocabSubMessage(_api.Configuration.CustomDictionaryPlainWords, _api.Configuration.CustomDictionarySoundsLikes);
            }

            var msg = new StartRecognitionMessage(_api.Configuration, audioFormat, additionalVocab);
            await msg.Send(_wsClient, _api.CancelToken);
        }

        public async Task SetRecognitionConfig()
        {
            var msg = new SetRecognitionConfigMessage(_api.Configuration);
            await msg.Send(_wsClient, _api.CancelToken);
        }
    }
}