namespace Speechmatics.Realtime.Client.Messages
{
    internal class DynamicTranscriptSubMessage
    {
        internal DynamicTranscriptSubMessage(DynamicTranscriptConfiguration config)
        {
            enabled = config.Enabled;
            max_chars = config.MaxChars;
            min_context = config.MinContext;
            max_delay = config.MaxDelay;
        }

        public bool enabled { get; }
        public int max_chars { get; }
        public double min_context { get; }
        public double max_delay { get; }
    }
}
 