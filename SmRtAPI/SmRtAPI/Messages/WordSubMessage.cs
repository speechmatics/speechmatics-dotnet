namespace Speechmatics.Realtime.Client.Messages
{
    /// <summary>
    /// Data for an individual word in a transcript message
    /// </summary>
    public class WordSubMessage : BaseMessage
    {
        /// <summary>
        /// Message type
        /// </summary>
        public override string message => "Word";
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