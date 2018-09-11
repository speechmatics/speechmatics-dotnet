using System;
using Newtonsoft.Json;
using Speechmatics.Realtime.Client;

namespace Speechmatics.Client.RtspExample
{
    public class Program
    {
        private static string ToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        private static string RtUrl => "wss://192.168.128.30:9000/";

        // ReSharper disable once UnusedParameter.Local
        public static void Main(string[] args)
        {
            var originalColor = Console.ForegroundColor;
            var x = Console.CursorLeft;
            var y = Console.CursorTop;

            using (var rtsp = new RtspStream("rtsp://192.168.128.30:8554/test"))
            {
                try
                {
                    rtsp.Go();
                    /*
                     * The API constructor is passed the websockets URL, callbacks for the messages it might receive,
                     * the language to transcribe (as a .NET CultureInfo object) and stream to read data from.
                     */
                    var config = new SmRtApiConfig("en")
                    {
                        AddTranscriptCallback = s =>
                        {
                            Console.SetCursorPosition(x, y);
                            Console.Write(s);
                            x = Console.CursorLeft;
                            y = Console.CursorTop;
                        },
                        AddPartialTranscriptMessageCallback = s =>
                        {
                            Console.SetCursorPosition(x, y);
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write(s.transcript);
                            Console.ForegroundColor = originalColor;
                        },
                        ErrorMessageCallback = s => Console.WriteLine(ToJson(s)),
                        WarningMessageCallback = s => Console.WriteLine(ToJson(s)),
                        Insecure = true,
                    };

                    var api = new SmRtApi(RtUrl,
                        rtsp,
                        config
                    );
                    // Run() will block until the transcription is complete.
                    Console.WriteLine($"Connecting to {RtUrl}");
                    api.Run();
                }
                catch (AggregateException e)
                {
                    Console.WriteLine(e);
                }
            }

            Console.WriteLine("End of stream");
            Console.ReadLine();
        }
    }
}
