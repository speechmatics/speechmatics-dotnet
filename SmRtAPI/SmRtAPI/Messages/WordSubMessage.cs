using System;
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
        public string type = String.Empty;
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
        public Alternative[] alternatives = Array.Empty<Alternative>();
        /// <summary>
        /// If the word is a named entity, the class of the entity (money, date, etc), otherwise omitted
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? entity_class;
        /// <summary>
        /// For an entity, the recognized spoken words, otherwise omitted
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public EntitySpokenOrWrittenForm[]? spoken_form;
        /// <summary>
        /// For an entity, the written form of the recognised words, otherwise omitted
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public EntitySpokenOrWrittenForm[]? written_form;
    }

    /// <summary>
    /// Represents the spoken or written form of an entity (money, date, etc)
    /// </summary>
    public class EntitySpokenOrWrittenForm
    {
        /// <summary>
        /// Alternative options for the words (currently of length 1)
        /// </summary>
        public Alternative[] alternatives = Array.Empty<Alternative>();
        /// <summary>
        /// Type for this token (usually 'word' when dealing with entity sub-components)
        /// </summary>
        public string type = String.Empty;
        /// <summary>
        /// Start time (offset from audio start) of the whole entity
        /// </summary>
        public double start_time;
        /// <summary>
        /// End time of the whole entity
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
        public string content = String.Empty;
        /// <summary>
        /// How confident the ASR is in the result
        /// </summary>
        public double confidence;
        /// <summary>
        /// The language. Currently this will be the same for all words
        /// </summary>
        public string language = String.Empty;

        /// <summary>
        /// Label indicating who said this word.
        /// </summary>
        public string? speaker;
    }
}
