namespace Speechmatics.Realtime.Client
{
    /// <summary>
    /// Represents the configuration for dynamic transcripts
    /// </summary>
    public class DynamicTranscriptConfiguration
    {
        public int MaxChars { get; }
        public double MinContext { get; }
        public double MaxDelay { get; }
        public bool Enabled { get; }

        /// <summary>
        /// Create configuration for dynamic transcripts
        /// </summary>
        /// <param name="enabled">Whether the feature is enabled</param>
        /// <param name="maxChars"></param>
        /// <param name="minContext"></param>
        /// <param name="maxDelay"></param>
        public DynamicTranscriptConfiguration(bool enabled, int maxChars, double minContext, double maxDelay)
        {
            MaxChars = maxChars;
            MinContext = minContext;
            MaxDelay = maxDelay;
            Enabled = enabled;
        }
    }
}