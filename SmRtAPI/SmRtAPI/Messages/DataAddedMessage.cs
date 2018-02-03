namespace SpeechmaticsAPI.Messages
{
    public class DataAddedMessage : BaseMessage
    {
        public override string message => "DataAdded";
        public int size;
        public int offset;
        public int seq_no;
    }
}