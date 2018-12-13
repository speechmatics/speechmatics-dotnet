namespace Speechmatics.Realtime.Client
{
    /// <summary>
    /// Represents the configuration for dynamic transcripts
    /// </summary>
    public class DynamicTranscriptConfiguration
    {
        /// <summary>
        ///  Whether defaults should be used
        /// </summary>
        public bool UseDefaults { get; }
        /// <summary>
        /// Maximum number of characters to wait before sending a transcript
        /// </summary>
        public int MaxChars { get; }
        /// <summary>
        /// Minimum amount of context to use
        /// </summary>
        public double MinContext { get; }
        /// <summary>
        /// Max delay between audio and transcript
        /// </summary>
        public double MaxDelay { get; }
        /// <summary>
        /// True if feature is enabled
        /// </summary>
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

        /// <summary>
        /// Default configuration for dynamic transcripts (values defined by server)
        /// </summary>
        /// <param name="enabled">Enable or disable the feature</param>
        public DynamicTranscriptConfiguration(bool enabled)
        {
            Enabled = enabled;
            UseDefaults = true;
        }
    }
}