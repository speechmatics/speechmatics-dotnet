using System;
using System.Collections.Generic;
using System.Text;

namespace Speechmatics.Realtime.Client.Messages
{
    public class PunctuationConfig
    {
        public List<string>? PermittedMarks { get; set; }

        public double? Sensitivity { get; set; }
    }
}
