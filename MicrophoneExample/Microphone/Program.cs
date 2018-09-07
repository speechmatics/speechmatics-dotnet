using System;
using System.Threading.Tasks;
using NAudio.Wave;
using Newtonsoft.Json;
using Speechmatics.Realtime.Client;
using Speechmatics.Realtime.Client.Enumerations;

namespace Speechmatics.Realtime.Microphone
{
    public class Program
    {
        private static readonly ProducerConsumerStream AudioStream = new ProducerConsumerStream();

        private static string ToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        private static string RtUrl => "wss://192.168.128.30:9000/";

        // ReSharper disable once UnusedParameter.Local
        public static void Main(string[] args)
        {
            var t = new Task(() =>
            {
                var waveSource = new WaveInEvent {WaveFormat = new WaveFormat(44100, 16, 1)};
                waveSource.DataAvailable += WaveSourceOnDataAvailable;
                waveSource.StartRecording();
            });
            t.Start();

            using (var stream = AudioStream)
            {
                try
                {
                    /*
                     * The API constructor is passed the websockets URL, callbacks for the messages it might receive,
                     * the language to transcribe (as a .NET CultureInfo object) and stream to read data from.
                     */
                    var config = new SmRtApiConfig("en", 44100, AudioFormatType.Raw, AudioFormatEncoding.PcmS16Le)
                    {
                        AddTranscriptCallback = Console.Write,
                        AddPartialTranscriptMessageCallback = s => Console.Write("* " + s.transcript),
                        ErrorMessageCallback = s => Console.WriteLine(ToJson(s)),
                        WarningMessageCallback = s => Console.WriteLine(ToJson(s)),
                        Insecure = true,
                    };

                    var api = new SmRtApi(RtUrl,
                        stream,
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

        private static void WaveSourceOnDataAvailable(object sender, WaveInEventArgs waveInEventArgs)
        {
            AudioStream.Write(waveInEventArgs.Buffer, 0, waveInEventArgs.BytesRecorded);
        }
    }
}
