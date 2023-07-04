using System;
using System.Collections.Generic;
using System.Text;

namespace Speechmatics.Realtime.Client.Config
{

    public class TranslationConfig {
        public IEnumerable<string> TargetLanguages { get; set; }

        public bool EnablePartials { get; set; } = false;

    }
}