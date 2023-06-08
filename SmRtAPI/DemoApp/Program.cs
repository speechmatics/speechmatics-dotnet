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
    /*
     This program is coded against an appliance using the v2 version of the API, releases 3.2.0 or above.
    
     To target a v1 appliance, change the V2 in `using Speechmatics.Realtime.Client.V2` and 
     `using Speechmatics.Realtime.Client.V2.Config` to a V1.
         
     The v2 appliances have a compatibility layer which talks v1 protocol -- v2 is under wss://host:9000/v2, v1 is under wss://host:9000/.
    */
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
            var language = Environment.GetEnvironmentVariable("LANG") ?? "en";
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
                        OutputLocale = "en-GB",
                        AddTranscriptCallback = s => builder.Append(s),
                        AddTranscriptMessageCallback = s => Console.WriteLine(ToJson(s)),
                        AddPartialTranscriptMessageCallback = s => Console.WriteLine(ToJson(s)),
                        ErrorMessageCallback = s => Console.WriteLine(ToJson(s)),
                        WarningMessageCallback = s => Console.WriteLine(ToJson(s)),
                        CustomDictionaryPlainWords = new[] {"speechmagic"},
                        CustomDictionarySoundsLikes = new Dictionary<string, IEnumerable<string>>(),
                        Insecure = true,
                        EnablePartials = true,
                        EnableEntities = true,
                        Diarization=DiarizationType.Speaker,
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