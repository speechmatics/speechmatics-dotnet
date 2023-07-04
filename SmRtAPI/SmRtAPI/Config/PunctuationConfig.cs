using System;
using System.Collections.Generic;
using System.Text;

namespace Speechmatics.Realtime.Client.Config
{
    public class PunctuationConfig
    {
        public IEnumerable<string>? PermittedMarks { get; set; }

        public double? Sensitivity { get; set; }
    }
}
