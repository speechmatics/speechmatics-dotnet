namespace Speechmatics.Realtime.Client.Messages
{
    public class AddPartialTranscriptMessage : BaseMessage
    {
        public override string message => "AddPartialTranscript";
        public double start_time;
        public double length;
        public string transcript;
        public WordSubMessage[] words;
    }
}