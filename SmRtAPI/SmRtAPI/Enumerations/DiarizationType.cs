using System;
using System.Collections.Generic;
using System.Text;

namespace Speechmatics.Realtime.Client.Enumerations
{
    /// <summary>
    /// Specifying the type of diarization for this session  
    /// </summary>
    public enum DiarizationType
    {
        /// <summary>
        /// No diarization
        /// </summary>
        None,
        /// <summary>
        /// Speaker diarization
        /// </summary>
        Speaker,
    }
}
