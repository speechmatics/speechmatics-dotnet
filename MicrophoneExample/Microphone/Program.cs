using System;
using System.Diagnostics;
using System.Threading.Tasks;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using Newtonsoft.Json;
using Speechmatics.Realtime.Client.Enumerations;
using Speechmatics.Realtime.Client.V2;
using Speechmatics.Realtime.Client.V2.Config;

namespace Speechmatics.Realtime.Microphone
{
    public class Program
    {
        private static readonly BlockingStream AudioStream = new BlockingStream(1024*1024*10);

        private static string ToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        private static string RtUrl => "wss://staging.realtimeappliance.speechmatics.io:9000/v2";

        // ReSharper disable once UnusedParameter.Local
        public static void Main(string[] args)
        {
            var t = new Task(() =>
            {
                // NAudio has two audio capture APIs

                // This is the first one I tried, but apparently the wasapi one is lower level and 
                // therefore better.
                //
                // var waveSource = new WaveInEvent {WaveFormat = new WaveFormat(44100, 16, 1)};
                // This is an example, but experiment shows that making the value too low will
                // result in incomplete buffers to send to the RT appliance, leading to bad
                // transcripts.
                // waveSource.BufferMilliseconds = 100;
                // waveSource.DataAvailable += WaveSourceOnDataAvailable;
                // waveSource.StartRecording();

                var wasapiClient = new WasapiCapture();
                Debug.WriteLine("Sample rate {0}", wasapiClient.WaveFormat.SampleRate);
                Debug.WriteLine("Bits per sample {0}", wasapiClient.WaveFormat.BitsPerSample);
                Debug.WriteLine("Channels {0}", wasapiClient.WaveFormat.Channels);
                Debug.WriteLine("Encoding {0}", wasapiClient.WaveFormat.Encoding);
                wasapiClient.DataAvailable += WaveSourceOnDataAvailable;
                wasapiClient.StartRecording();
            });
            t.Start();

            using (var stream = AudioStream)
            {
                try
                {
                    /*
                     * The API constructor is passed the websockets URL, callbacks for the messages it might receive,
                     * the language to transcribe and stream to read data from.
                     */

                    // Make sure the sampleRate matches the value in the wasapiClient object
                    var config = new SmRtApiConfig("en", 16000, AudioFormatType.Raw, AudioFormatEncoding.PcmF32Le)
                    {
                        AddTranscriptCallback = Console.Write,
                        // AddPartialTranscriptMessageCallback = s => Console.Write("* " + s.transcript),
                        ErrorMessageCallback = s => Console.WriteLine(ToJson(s)),
                        WarningMessageCallback = s => Console.WriteLine(ToJson(s)),
                        Insecure = true,
                        BlockSize = 16384
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
