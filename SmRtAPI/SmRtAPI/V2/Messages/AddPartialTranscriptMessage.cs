using Speechmatics.Realtime.Client.Messages;


namespace Speechmatics.Realtime.Client.V2.Messages
{
    /// <summary>
    /// A partial transcript -- later messages may improve on it
    /// </summary>
    public class AddPartialTranscriptMessage : BaseMessage
    {
        /// <summary>
        /// Message type
        /// </summary>
        public string message => "AddPartialTranscript";

        /// <summary>
        /// Aggregate metadata for the whole message
        /// </summary>
        public RecognitionMetadata metadata;
        /// <summary>
        /// Individual word data
        /// </summary>
        public WordSubMessage[] results;
    }
}