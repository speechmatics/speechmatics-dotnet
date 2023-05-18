using Speechmatics.Realtime.Client.Messages;

namespace Speechmatics.Realtime.Client.V2.Messages
{
    /// <summary>
    /// Detailed timings for a transcript
    /// </summary>
    public class AddTranscriptMessage : BaseMessage
    {
        /// <summary>
        /// Message type
        /// </summary>
        public override string message => "AddTranscript";
        /// <summary>
        /// Individual word data
        /// </summary>
        public WordSubMessage[] results;
        /// <summary>
        /// Aggregate metadata for the whole message.
        /// </summary>
        public RecognitionMetadata metadata;
    }

    /// <summary>
    /// Metadata that aggregates information for a whole parital or final message.
    /// </summary>
    public class RecognitionMetadata : BaseMessage
    {
        /// <summary>
        /// Message Type
        /// </summary>
        public override string message => "Metadata";

        /// <summary>
        /// Concatenated tokens for this message
        /// </summary>
        public string transcript;

        /// <summary>
        /// Start time of first token in seconds.
        /// </summary>
        public float start_time;

        /// <summary>
        /// End time of last token in seconds.
        /// </summary>
        public float end_time;
    }
}