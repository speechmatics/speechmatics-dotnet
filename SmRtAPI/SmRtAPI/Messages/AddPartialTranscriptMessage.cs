using System;

namespace Speechmatics.Realtime.Client.Messages
{
    /// <summary>
    /// A partial transcript is a transcript that can be changed and expanded by a future AddTranscript or AddPartialTranscript message 
    /// corresponds to the part of audio since the last AddTranscript message. 
    /// For AddPartialTranscript messages the confidence field for alternatives has no meaning and will always be equal to 0.
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
        public RecognitionMetadata? metadata;
        /// <summary>
        /// Individual word data
        /// </summary>
        public WordSubMessage[] results = Array.Empty<WordSubMessage>();
    }
}