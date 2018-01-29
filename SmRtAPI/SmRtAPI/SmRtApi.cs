using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using SpeechmaticsAPI.Messages;

namespace SpeechmaticsAPI
{
    /*
     *   * `model` (String): language product used to process the job (for example `en-US`)
  * `audio_format` (Object:AudioFormatEncoding): audio stream type you the user is going to send: see [Supported audio types](#supported-audio-types).
  * `output_format` (Object:OutputFormat): Requested output format, see [Supported output formats](#supported-output-formats).

    */

    public enum AudioFormatEncoding
    {
        PcmF32Le,
        PcmS16Le,
        File
    }

    public enum OutputFormat
    {
        Ttxt,
        Json
    }

    public enum AudioFormatType
    {
        Raw,
        File,
        Opus
    }

    public class SmRtApi
    {
        private readonly ClientWebSocket _wsClient;
        private readonly CancellationToken _cancellationToken;
        public Uri WsUrl { get; }

        public SmRtApi(string wsUrl)
        {
            WsUrl = new Uri(wsUrl);
            _wsClient = new ClientWebSocket();
            _cancellationToken = new CancellationToken();
        }

        public Task Connect()
        {
            return _wsClient.ConnectAsync(WsUrl, _cancellationToken);
        }

        public void StartRecognition()
        {

        }
    }
}
