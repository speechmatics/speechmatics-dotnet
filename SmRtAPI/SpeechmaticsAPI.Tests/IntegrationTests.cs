using System;
using System.Globalization;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace SpeechmaticsAPI.Tests
{
    [TestFixture]
    public class IntegrationTests
    {
        private const string SampleAudio = "2013-8-british-soccer-football-commentary-alex-warner.mp3";

        [Test]
        public void RunAgainstLive()
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
                    var api = new SmRtApi("wss://api.rt.speechmatics.io:9000/",
                        s => builder.Append(s),
                        CultureInfo.GetCultureInfo("en-US"),
                        stream
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