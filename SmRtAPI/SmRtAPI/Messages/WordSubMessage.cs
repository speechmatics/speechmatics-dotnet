using Newtonsoft.Json;

namespace Speechmatics.Realtime.Client.Messages
{
    /// <summary>
    /// Data for an individual word in a transcript message
    /// </summary>
    public class WordSubMessage : BaseMessage
    {
        /// <summary>
        /// Type for this token, one of "word", "punctuation" or "entity"
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
        /// Alternative options for the words (currently of length 1)
        /// </summary>
        public Alternative[] alternatives;
        /// <summary>
        /// If the word is a named entity, the class of the entity (money, date, etc)
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? entity_class;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public EntitySpokenOrWrittenForm[]? spoken_form;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public EntitySpokenOrWrittenForm[]? written_form;
    }

    public class EntitySpokenOrWrittenForm
    {
        public Alternative[] alternatives;
        /// <summary>
        /// Type for this token, one of "word", "punctuation" or "entity"
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
    }

    /// <summary>
    /// An alternative for a word
    /// </summary>
    public class Alternative
    {
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
