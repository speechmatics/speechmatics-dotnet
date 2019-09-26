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
    }
}