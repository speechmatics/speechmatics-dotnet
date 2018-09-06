namespace Speechmatics.Realtime.Client.Messages
{
#pragma warning disable CS0649 // 'field never set' - it looks this way but they get set by JSON.NET during deserialization
    internal class DataAddedMessage : BaseMessage
    {
        public override string message => "DataAdded";
        public int size;
        public int offset;
        public int seq_no;
    }
}