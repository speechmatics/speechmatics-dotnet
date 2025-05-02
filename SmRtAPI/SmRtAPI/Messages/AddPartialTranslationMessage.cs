using System;

namespace Speechmatics.Realtime.Client.Messages
{

    /// <summary>
    /// A partial translation is a translation that can be changed and expanded by a future AddTranslation or AddPartialTranslation message 
    /// and corresponds to the part of audio since the last AddTranslation message.
    /// </summary>
    public class AddPartialTranslationMessage : BaseMessage
    {
        /// <summary>
        /// Message type
        /// </summary>
        public string message => "AddPartialTranslation";

        /// <summary>
        /// Target language translation relates to
        /// </summary>
        public string? language;

        /// <summary>
        /// List of translated sentences.
        /// </summary>
        public TranslationSubMessage[] results = Array.Empty<TranslationSubMessage>();
    }
}