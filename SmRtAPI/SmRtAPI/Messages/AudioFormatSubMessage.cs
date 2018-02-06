using Speechmatics.Realtime.Client.Enumerations;

namespace Speechmatics.Realtime.Client.Messages
{
    public class AudioFormatSubMessage
    {
        //   `AudioFormatSubMessage: {type: "raw", encoding: "pcm_s16le", sample_rate: 44100}`
        public AudioFormatSubMessage(AudioFormatType audioFormatType, AudioFormatEncoding encoding, int sampleRate)
        {
            type = audioFormatType.ToApiString();
            this.encoding = encoding.ToApiString();
            sample_rate = sampleRate;
        }

        public int sample_rate { get; }
        public string type { get; }
        public string encoding { get; }
    }
}