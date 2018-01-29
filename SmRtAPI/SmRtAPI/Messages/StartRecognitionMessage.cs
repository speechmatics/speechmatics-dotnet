using SpeechmaticsAPI;

namespace SpeechmaticsAPI.Messages
{
    // {"message":"EndOfStream","last_seq_no":10}

    public class StartRecognitionMessage : BaseMessage
    {
        private readonly string _model;
        private readonly string _outputFormat;
        private readonly AudioFormat _audioFormat;
        private readonly string _authToken;
        private readonly int _user;

        public StartRecognitionMessage(AudioFormat audioFormat, string model, OutputFormat outputFormat, string authToken = "", int user = 1)
        {
            _audioFormat = audioFormat;
            _model = model;
            _outputFormat = outputFormat.ToApiString();
            _authToken = authToken;
            _user = user;
        }

        public override string message => "StartRecognition";
        public string model => _model;
        public AudioFormat audio_format => _audioFormat;
        public string output_format => _outputFormat;
        public string auth_token => _authToken;
        public int user => _user;
    }
}