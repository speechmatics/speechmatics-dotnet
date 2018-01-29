using System;
using NUnit.Framework;
using SpeechmaticsAPI.Messages;

namespace SpeechmaticsAPI.Tests
{
    [TestFixture]
    public class MessagingTests
    {
        [Test]
        public void Constructor()
        {
            var url = "wss://this/";
            var s = new SmRtApi(url);
            Assert.AreEqual(url, s.WsUrl.AbsoluteUri, "Get WS url back");
        }

        [Test]
        public void StartRecognitionToJson()
        {
            // This is just a sanity check for now
            var expected = "{\"message\":\"StartRecognition\",\"model\":\"en-US\",\"audio_format\":{\"sample_rate\":44100,\"type\":\"raw\",\"encoding\":\"pcm_s16le\"},\"output_format\":\"json\",\"auth_token\":\"a\",\"user\":1}";
            var audioFormat = new AudioFormat(AudioFormatType.Raw, AudioFormatEncoding.PcmS16Le, 44100);
            var msg = new StartRecognitionMessage(audioFormat, "en-US", OutputFormat.Json);
            Assert.AreEqual(expected, msg.AsJson(), "Message serialization unexpected");
        }
    
    }
}
