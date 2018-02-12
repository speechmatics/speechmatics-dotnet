using System;
using System.Globalization;
using System.IO;
using System.Text;
using NUnit.Framework;
using Speechmatics.Realtime.Client;

namespace SpeechmaticsAPI.Tests
{
    [TestFixture]
    public class IntegrationTests
    {
        private const string SampleAudio = "2013-8-british-soccer-football-commentary-alex-warner.mp3";

        [TestCase("en-US")]
        public void RunAgainstLive(string language)
        {
            RunTest(language);
        }

        [TestCase("en-US")]
        [TestCase("en-AU")]
        [TestCase("en-GB")]
        [TestCase("it")]
        [TestCase("nl")]
        [TestCase("de")]
        [TestCase("ja")]
        [TestCase("es")]
        [TestCase("fr")]
        [TestCase("ru")]
        [TestCase("sv")]
        [Explicit]
        public void RunAgainstLiveAllLanguages(string language)
        {
            RunTest(language);
        }

        private static void RunTest(string language)
        {
            string expectedTranscript;
            var sampleAudioSource = Path.Combine(TestContext.CurrentContext.TestDirectory, SampleAudio);
            using (var transcript =
                File.OpenText(Path.Combine(TestContext.CurrentContext.TestDirectory, "expected_transcript.txt")))
            {
                expectedTranscript = transcript.ReadToEnd();
            }

            var builder = new StringBuilder();

            using (var stream = File.Open(sampleAudioSource, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    var config = new SmRtApiConfig(language) {AddTranscriptCallback = s => builder.Append(s)};

                    var api = new SmRtApi("wss://api.rt.speechmatics.io:9000/",
                        stream,
                        config
                    );
                    // Run() will block until the transcription is complete.
                    api.Run();
                    Console.WriteLine(builder.ToString());
                }
                catch (AggregateException e)
                {
                    Console.WriteLine(e);
                    throw;
                }

                // TODO: Word error rate comparison
                //Assert.AreEqual(expectedTranscript, builder.ToString(), "Unexpected transcript in the bagging area");
            }
        }
    }
}