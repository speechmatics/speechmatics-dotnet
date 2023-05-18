using Speechmatics.Realtime.Client.Messages;

namespace Speechmatics.Realtime.Client.V1.Messages
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
        /// Start time
        /// </summary>
        public double start_time;
        /// <summary>
        /// Length of audio segment
        /// </summary>
        public double length;
        /// <summary>
        /// Transcript ext
        /// </summary>
        public string transcript;
        /// <summary>
        /// Individual word data
        /// </summary>
        public WordSubMessage[] words;
    }
}