namespace Speechmatics.Realtime.Client.Messages
{
    internal class AddDataMessage : BaseMessage
    {
        public override string message => "AddData";
        public int size;
        public int offset;
        public int seq_no;
    }
}
 