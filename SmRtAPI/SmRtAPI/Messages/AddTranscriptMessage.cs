namespace Speechmatics.Realtime.Client.Messages
{
    /// <summary>
    /// Detailed timings for a transcript
    /// </summary>
    public class AddTranscriptMessage : BaseMessage
    {
        public override string message => "AddTranscript";
        public double start_time;
        public double length;
        public string transcript;
        public WordSubMessage[] words;
    }
}