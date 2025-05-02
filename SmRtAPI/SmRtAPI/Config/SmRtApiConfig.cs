using System;
using Speechmatics.Realtime.Client.Enumerations;
using Speechmatics.Realtime.Client.Messages;

namespace Speechmatics.Realtime.Client.Config
{
    /// <summary>
    /// Configuration for an RT API session
    /// </summary>
    public class SmRtApiConfig : SmRtApiConfigBase
    {
        /// <summary>
        /// Action to perform on extended partial transcript data
        /// </summary>
        public Action<AddPartialTranscriptMessage>? AddPartialTranscriptMessageCallback { get; set; }

        /// <summary>
        /// Action to perform on extended transcript data
        /// </summary>
        public Action<AddTranscriptMessage>? AddTranscriptMessageCallback { get; set; }

        /// <summary>
        /// Action to perform on partial translation data
        /// </summary>
        public Action<AddPartialTranslationMessage>? AddPartialTranslationMessageCallback { get; set; }

        /// <summary>
        /// Action to perform on translation data
        /// </summary>
        public Action<AddTranslationMessage>? AddTranslationMessageCallback { get; set; }

        /// <summary>
        /// Create new config for a given language,
        /// specifying input audio details
        /// </summary>
        /// <param name="model"></param>
        /// <param name="sampleRate"></param>
        /// <param name="audioFormatType"></param>
        /// <param name="audioFormatEncoding"></param>
        public SmRtApiConfig(string model, int sampleRate, AudioFormatType audioFormatType, AudioFormatEncoding audioFormatEncoding) : base(model, sampleRate, audioFormatType, audioFormatEncoding)
        {
        }

        /// <summary>
        /// Create transcription config for the specified language
        /// </summary>
        /// <param name="model"></param>
        public SmRtApiConfig(string model) : base(model)
        {
        }
    }
}