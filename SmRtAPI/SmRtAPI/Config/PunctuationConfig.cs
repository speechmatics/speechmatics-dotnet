using System;
using System.Collections.Generic;
using System.Text;

namespace Speechmatics.Realtime.Client.Config
{

    /// <summary>
    /// Configuration for customizing punctuation
    /// </summary>
    public class PunctuationConfig
    {
        /// <summary>
        /// List of permitted punctuation marks
        /// </summary>
        public IEnumerable<string>? PermittedMarks { get; set; }

        /// <summary>
        /// Punctuation detection sensitivity, higher values will produce more punctuation, the default is 0.5
        /// </summary>
        public double? Sensitivity { get; set; }
    }
}
