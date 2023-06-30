using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Speechmatics.Realtime.Client;
using Speechmatics.Realtime.Client.Config;
using Newtonsoft.Json;
using Speechmatics.Realtime.Client.Enumerations;

namespace DemoApp
{

    public class Program
    {
        private const string SampleAudio = "2013-8-british-soccer-football-commentary-alex-warner.mp3";

        private static string ToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        private static string RtUrl
        {
            get
            {
                return "wss://neu.rt.speechmatics.com/v2/en";
                var host = Environment.GetEnvironmentVariable("TEST_HOST") ?? "api.rt.speechmatics.io";
                return host.StartsWith("wss://") ? host : $"wss://{host}:9000/";
            }
        }

        // ReSharper disable once UnusedParameter.Local
        public static void Main(string[] args)
        {
            var start = DateTime.Now;
            Debug.WriteLine("Starting at {0}", start);
            var builder = new StringBuilder();
            var language = Environment.GetEnvironmentVariable("SM_LANG") ?? "en";
            Console.WriteLine(language);

            using (var stream = File.Open(SampleAudio, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    /*
                     * The API constructor is passed the websockets URL, callbacks for the messages it might receive,
                     * the language to transcribe and stream to read data from.
                     */
                    var config = new SmRtApiConfig(language)
                    {
                        AuthToken = Environment.GetEnvironmentVariable("SM_TOKEN"),
                        // GenerateTempToken = True <- set this to True for accounts from portal.speechmatics.com
                        // AddTranscriptCallback = s => builder.Append(s),
                        AddTranscriptMessageCallback = s => Console.WriteLine(ToJson(s)),
                        AddPartialTranscriptMessageCallback = s => Console.WriteLine(ToJson(s)),
                        AddTranslationMessageCallback = s => Console.WriteLine(ToJson(s)),
                        AddPartialTranslationMessageCallback = s => Console.WriteLine(ToJson(s)),
                        ErrorMessageCallback = s => Console.WriteLine(ToJson(s)),
                        WarningMessageCallback = s => Console.WriteLine(ToJson(s)),
                        CustomDictionaryPlainWords = new[] {"speechmagic"},
                        CustomDictionarySoundsLikes = new Dictionary<string, IEnumerable<string>>(),
                        Insecure = true,
                        EnablePartials = true,
                        EnableEntities = true,
                        Diarization=DiarizationType.Speaker,
                        TranslationConfig = new TranslationConfig() {
                            TargetLanguages = new [] {"de"},
                            EnablePartials = true
                        }
                    };

                    // We can do this here, or earlier. It's not used until .Run() is called on the API object.
                    config.CustomDictionarySoundsLikes["gnocchi"] = new[] {"nokey", "noki"};

                    var api = new SmRtApi(RtUrl,
                        stream,
                        config
                    );
                    // Run() will block until the transcription is complete.
                    Console.WriteLine($"Connecting to {RtUrl}");
                    api.Run();
                    Console.WriteLine(builder.ToString());
                }
                catch (AggregateException e)
                {
                    Console.WriteLine(e);
                }
            }

            var finish = DateTime.Now;
            Debug.WriteLine("Starting at {0} -- {1}", finish, finish-start);
            Console.ReadLine();
        }
    }
}
