namespace Speechmatics.Realtime.Client.Messages
{
    internal class EndOfTranscriptMessage : BaseMessage
    {
        public string message => "EndOfTranscript";
    }
}