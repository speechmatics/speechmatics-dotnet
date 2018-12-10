namespace Speechmatics.Realtime.Client.Messages
{
    internal class DynamicTranscriptSubMessage
    {
        internal DynamicTranscriptSubMessage(bool enabled)
        {
            this.enabled = enabled;
        }

        public bool enabled { get; }
    }
}