using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http.Headers;
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
        private static bool IsStereo;
        private static readonly BlockingStream AudioStream = new BlockingStream(1024*1024);
        private static string ToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        // private static string RtUrl => "wss://staging.realtimeappliance.speechmatics.io:9000/v2";
        private static string RtUrl => "ws://192.168.128.30:9000/v2";

        private static MMDevice ChooseDevice()
        {
            var devices = new MMDeviceEnumerator().EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);
            Console.WriteLine("Detected devices - enter selection");
            int idx = 1;
            foreach (var dev in devices)
            {
                Console.WriteLine($"{idx++}) {dev}");
            }
            Console.WriteLine();

            while (true)
            {
                var input = Console.ReadLine();
                try
                {
                    var choice = Int32.Parse(input);
                    return devices[choice - 1];
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        // ReSharper disable once UnusedParameter.Local
        public static void Main(string[] args)
        {
            // NAudio has two (three?) audio capture APIs

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

            var selectedDevice = ChooseDevice();

            var wasapiClient = new WasapiCapture(selectedDevice);
            var sampleRate = wasapiClient.WaveFormat.SampleRate;
            var channels = wasapiClient.WaveFormat.Channels;

            IsStereo = channels == 2;

            Console.WriteLine("Sample rate {0}", sampleRate);
            Console.WriteLine("Bits per sample {0}", wasapiClient.WaveFormat.BitsPerSample);
            Console.WriteLine("Channels {0}", channels);
            Console.WriteLine("Encoding {0}", wasapiClient.WaveFormat.Encoding);
            wasapiClient.DataAvailable += WaveSourceOnDataAvailable;

            var t = new Task(() =>
            {
                wasapiClient.StartRecording();
            });

            using (var stream = AudioStream)
            {
                try
                {
                    /*
                     * The API constructor is passed the websockets URL, callbacks for the messages it might receive,
                     * the language to transcribe and stream to read data from.
                     */

                    // Make sure the sampleRate matches the value in the wasapiClient object
                    var config = new SmRtApiConfig("en", sampleRate, AudioFormatType.Raw, AudioFormatEncoding.PcmF32Le)
                    {
                        AddTranscriptCallback = Console.Write,
                        // AddPartialTranscriptMessageCallback = s => Console.Write("* " + s.transcript),
                        ErrorMessageCallback = s => Console.WriteLine(ToJson(s)),
                        WarningMessageCallback = s => Console.WriteLine(ToJson(s)),
                        Insecure = true,
                        BlockSize = 8192
                    };

                    var api = new SmRtApi(RtUrl,
                        stream,
                        config
                    );

                    // Start recording audio
                    t.Start();

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

        /// <summary>
        /// We want to turn the 2 streams into 1.
        /// </summary>
        /// <param name="waveInEventArgs"></param>
        /// <returns></returns>
        private static byte[] SquashStereo(WaveInEventArgs waveInEventArgs)
        {
            var data = waveInEventArgs.Buffer;
            // TODO: do not allocate a block every time this is called
            var audioBytes = new byte[waveInEventArgs.BytesRecorded / 2];
            // offset into the receiving buffer
            var offset = 0;

            for (var i = 0; i < waveInEventArgs.BytesRecorded; i += 8)
            {
                var s = BitConverter.ToSingle(data, i) / 2.0f + BitConverter.ToSingle(data, i + 4) / 2.0f;
                Buffer.BlockCopy(BitConverter.GetBytes(s), 0, audioBytes, offset, 4);
                offset += 4;
            }

            return audioBytes;
        }

        private static void WaveSourceOnDataAvailable(object sender, WaveInEventArgs waveInEventArgs)
        {

            // Use this code to save the audio to a .raw file to examine in Audacity
            //
            //using (var f = File.OpenWrite("./audio.raw"))
            //{
            //    f.Seek(0, SeekOrigin.End);
            //    f.Write(waveInEventArgs.Buffer, 0, waveInEventArgs.BytesRecorded);
            //    //f.Write(squashed, 0, squashed.Length);
            //}
            if (IsStereo)
            {
                var squashed = SquashStereo(waveInEventArgs);
                AudioStream.Write(squashed, 0, squashed.Length);
            }
            else
            {
                AudioStream.Write(waveInEventArgs.Buffer, 0, waveInEventArgs.BytesRecorded);
            }
        }
    }
}
