using System;
using System.Collections.Generic;
using System.Text;

namespace Speechmatics.Realtime.Client.Config
{

    /// <summary>
    /// Additional configuration for translation
    /// </summary>
    public class TranslationConfig {

        /// <summary>
        /// List of target languages to translate to
        /// </summary>
        public IEnumerable<string> TargetLanguages { get; set; }

        /// <summary>
        /// Flag for enabling translation partials.
        /// These are based on transcription partials and thus subject to change.
        /// </summary>
        public bool EnablePartials { get; set; } = false;

    }
}