using System;
using System.Globalization;
using System.IO;
using System.Text;
using SpeechmaticsAPI;

namespace DemoApp
{
    class Program
    {
        // ReSharper disable once UnusedParameter.Local
        static void Main(string[] args)
        {
            var builder = new StringBuilder();
            // wss://api.rt.speechmatics.io:9000/
            using (var stream = File.Open("2013-8-british-soccer-football-commentary-alex-warner.mp3", FileMode.Open,
                FileAccess.Read))
            {
                try
                {
                    var api = new SmRtApi("wss://api.rt.speechmatics.io:9000/",
                        s => builder.Append(s),
                        CultureInfo.GetCultureInfo("en-US"),
                        stream
                        );
                    api.Run();
                    Console.WriteLine(builder.ToString());

                }
                catch (AggregateException e)
                {
                    Console.WriteLine(e);
                }
            }

            Console.ReadLine();
        }
    }
}
