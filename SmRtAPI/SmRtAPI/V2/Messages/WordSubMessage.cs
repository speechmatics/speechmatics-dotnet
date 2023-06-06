using Speechmatics.Realtime.Client.Messages;

namespace Speechmatics.Realtime.Client.V2.Messages
{
    /// <summary>
    /// Data for an individual word in a transcript message
    /// </summary>
    public class WordSubMessage : BaseMessage
    {
        /// <summary>
        /// Type for this token, one of "word", "punctuation" or "speaker_change"
        /// </summary>
        public string type;
        /// <summary>
        /// Start time (offset from audio start)
        /// </summary>
        public double start_time;
        /// <summary>
        /// End time
        /// </summary>
        public double end_time;

        /// <summary>
        /// Whether the element represents an end of sentence marker (e.g. full stop, question mark, exclamation mark)
        /// </summary>
        public bool is_eos;

        /// <summary>
        /// Whether this punctuation mark attaches to previous or next token.
        /// </summary>
        public string? attaches_to;
        /// <summary>
        /// Audio length (seconds)
        /// </summary>
        public Alternative[] alternatives;
    }

    /// <summary>
    /// An alternative for a word
    /// </summary>
    public class Alternative
    {
        /// <summary>
        /// The type of object, "word" or "punctuation"
        /// </summary>
        public string type;
        /// <summary>
        /// The content, e.g. "hello" (a word) or "," (a punctuation mark)
        /// </summary>
        public string content;
        /// <summary>
        /// How confident the ASR is in the result
        /// </summary>
        public double confidence;
        /// <summary>
        /// The language. Currently this will be the same for all words
        /// </summary>
        public string language;


        /// <summary>
        /// Label indicating who said this word.
        /// </summary>
        public string? speaker;
    }
}
