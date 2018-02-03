using System;
using System.Globalization;
using System.IO;
using SpeechmaticsAPI;

namespace DemoApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // wss://api.rt.speechmatics.io:9000/
            using (var stream = File.Open("2013-8-british-soccer-football-commentary-alex-warner.mp3", FileMode.Open,
                FileAccess.Read))
            {
                try
                {
                    var api = new SmRtApi("wss://api.rt.speechmatics.io:9000/", CultureInfo.GetCultureInfo("en-US"),
                        stream);
                    api.Run();
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
