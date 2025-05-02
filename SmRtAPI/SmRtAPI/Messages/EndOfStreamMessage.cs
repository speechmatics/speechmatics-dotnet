namespace Speechmatics.Realtime.Client.Messages
{
    internal class EndOfStreamMessage : BaseMessage
    {
        public EndOfStreamMessage(int lastSequenceNumber)
        {
            last_seq_no = lastSequenceNumber;
        }

        public int last_seq_no { get; }
        public string message => "EndOfStream";
    }
}