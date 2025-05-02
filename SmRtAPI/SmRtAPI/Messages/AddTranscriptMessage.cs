using System;

namespace Speechmatics.Realtime.Client.Messages
{
    /// <summary>
    /// This message is sent from the Server to the Client, and contains part of the transcript. 
    /// Each message corresponds to the audio since the last AddTranscript message. 
    /// These messages are also referred to as Finals since the transcript will not change any further. 
    /// An AddTranscript message is sent when we reach an endpoint (end of a sentence or a phrase in the audio), 
    /// or after the max_delay.
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
        public WordSubMessage[] results = Array.Empty<WordSubMessage>();
        /// <summary>
        /// Aggregate metadata for the whole message
        /// </summary>
        public RecognitionMetadata? metadata;
    }
}