using System;
using System.Collections.Generic;
using System.Text;

namespace Speechmatics.Realtime.Client.Config
{
    /// <summary>
    /// Additional configuration for diarization
    /// </summary>
    public class SpeakerDiarizationConfig
    {
        /// <summary>
        /// This enforces the maximum number of speakers allowed in a single audio stream
        /// </summary>
        public int? MaxSpeakers { get; set; }
    }
}
