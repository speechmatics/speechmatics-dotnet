namespace Speechmatics.Realtime.Client.Messages
{
    internal class WarningMessage : BaseMessage
    {
        public string reason;
        public override string message => "Warning";
        public string type;
        public float duration_limit;
    }
}