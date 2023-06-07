namespace Speechmatics.Realtime.Client.Messages
{
    /// <summary>
    /// Detailed timings for a transcript
    /// </summary>
    public class AddTranscriptMessage : BaseMessage
    {
        /// <summary>
        /// Message type
        /// </summary>
        public string message => "AddTranscript";
        /// <summary>
        /// Individual word data
        /// </summary>
        public WordSubMessage[] results;
        /// <summary>
        /// Aggregate metadata for the whole message
        /// </summary>
        public RecognitionMetadata metadata;
    }
}