using System.IO;
using NUnit.Framework;
using Speechmatics.Realtime.Client.Enumerations;
using Speechmatics.Realtime.Client.Messages;
using Speechmatics.Realtime.Client;
using Speechmatics.Realtime.Client.Config;

namespace SpeechmaticsAPI.Tests
{
    [TestFixture]
    public class EncodingTests
    {
        [Test]
        public void Constructor()
        {
            var url = "wss://this/";
            var s = new SmRtApi(url, Stream.Null, new SmRtApiConfig("ru"));
            Assert.AreEqual(url, s.WsUrl.AbsoluteUri, "Get WS url back");
        }

        [Test]
        public void StartRecognitionV2ToJson()
        {
            // This is just a sanity check for now
            var expected = "{\"message\":\"StartRecognition\",\"audio_format\":{\"sample_rate\":44100,\"type\":\"raw\",\"encoding\":\"pcm_s16le\"},\"transcription_config\":{\"language\":\"en-US\"}}";
            var audioFormat = new AudioFormatSubMessage(AudioFormatType.Raw, AudioFormatEncoding.PcmS16Le, 44100);
            var msg = new StartRecognitionMessage(new SmRtApiConfig("en"), audioFormat, null);
            Assert.AreEqual(expected, msg.AsJson(), "Message serialization unexpected");
        }

        [Test]
        public void SetRecognitionConfigToJson()
        {
            // This is just a sanity check for now
            var expected =
            "{\"message\":\"SetRecognitionConfig\",\"transcription_config\":{\"language\":\"en\", \"max_delay\": 7}}";

            var config = new SmRtApiConfig("en");
            config.MaxDelay = 7;
            var msg = new SetRecognitionConfigMessage(config);
            var y = msg.AsJson();
            Assert.AreEqual(expected, msg.AsJson(), "Message serialization unexpected");
        }
    }
}
