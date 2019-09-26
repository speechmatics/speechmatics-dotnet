using Speechmatics.Realtime.Client.Messages;

namespace Speechmatics.Realtime.Client.V2.Messages
{
    internal class StartRecognitionMessage : BaseMessage
    {
        public StartRecognitionMessage(AudioFormatSubMessage audioFormatSubMessage, string model)
        {
            audio_format = audioFormatSubMessage;
            transcription_config = new { language = model };
        }

        public override string message => "StartRecognition";
        public AudioFormatSubMessage audio_format { get; }
        public object transcription_config { get; }
    }
}