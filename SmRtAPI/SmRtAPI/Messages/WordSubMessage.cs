namespace Speechmatics.Realtime.Client.Messages
{
    public class WordSubMessage : BaseMessage
    {
        public override string message => "Word";
        public string word;
        public double start_time;
        public double length;
    }
}