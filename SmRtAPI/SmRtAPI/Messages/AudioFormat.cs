namespace SpeechmaticsAPI.Messages
{
    public class AudioFormat
    {
        private readonly string _audioFormatType;
        private readonly string _encoding;
        private readonly int _sampleRate;

        //   `audio_format: {type: "raw", encoding: "pcm_s16le", sample_rate: 44100}`
        public AudioFormat(AudioFormatType audioFormatType, AudioFormatEncoding encoding, int sampleRate)
        {
            _audioFormatType = audioFormatType.ToApiString();
            _encoding = encoding.ToApiString();
            _sampleRate = sampleRate;
        }

        public int sample_rate => _sampleRate;
        public string type => _audioFormatType;
        public string encoding => _encoding;

    }
}