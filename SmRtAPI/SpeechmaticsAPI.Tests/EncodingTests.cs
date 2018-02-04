using System.Globalization;
using System.IO;
using NUnit.Framework;
using Speechmatics.Realtime.Client;
using Speechmatics.Realtime.Client.Enumerations;
using Speechmatics.Realtime.Client.Messages;

namespace SpeechmaticsAPI.Tests
{
    [TestFixture]
    public class EncodingTests
    {
        [Test]
        public void Constructor()
        {
            var url = "wss://this/";
            var s = new SmRtApi(url, _ => { }, CultureInfo.CurrentCulture, Stream.Null);
            Assert.AreEqual(url, s.WsUrl.AbsoluteUri, "Get WS url back");
        }

        [Test]
        public void StartRecognitionToJson()
        {
            // This is just a sanity check for now
            var expected =
                "{\"message\":\"StartRecognition\",\"model\":\"en-US\",\"audio_format\":{\"sample_rate\":44100,\"type\":\"raw\",\"encoding\":\"pcm_s16le\"},\"output_format\":{\"type\":\"json\"},\"auth_token\":\"\",\"user\":1}";
            var audioFormat = new AudioFormatSubMessage(AudioFormatType.Raw, AudioFormatEncoding.PcmS16Le, 44100);
            var msg = new StartRecognitionMessage(audioFormat, "en-US", OutputFormat.Json);
            Assert.AreEqual(expected, msg.AsJson(), "Message serialization unexpected");
        }
    }
}
