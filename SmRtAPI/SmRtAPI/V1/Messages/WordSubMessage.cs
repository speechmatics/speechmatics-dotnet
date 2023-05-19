using Speechmatics.Realtime.Client.Messages;

namespace Speechmatics.Realtime.Client.V1.Messages
{
    /// <summary>
    /// Data for an individual word in a transcript message
    /// </summary>
    public class WordSubMessage : BaseMessage
    {
        /// <summary>
        /// Word text
        /// </summary>
        public string word;
        /// <summary>
        /// Start time (offset from audio start)
        /// </summary>
        public double start_time;
        /// <summary>
        /// Audio length (seconds)
        /// </summary>
        public double length;
    }
}