using System;
using System.Globalization;
using Speechmatics.Realtime.Client.Enumerations;

namespace Speechmatics.Realtime.Client
{
    /// <summary>
    /// Configuration for an SmRtApi session
    /// </summary>
    public class SmRtApiConfig
    {
        /// <summary>
        /// Language, e.g. en-US, ru, de
        /// </summary>
        public string Model { get; internal set; }
        /// <summary>
        /// Audio sample rate, e.g. 16000 (for 16kHz), 44100 (for 44.1kHz CD quality)
        /// </summary>
        public int SampleRate { get; internal set; }
        /// <summary>
        /// Enum of File or Raw
        /// </summary>
        public AudioFormatType AudioFormat { get; internal set; }
        /// <summary>
        /// If AudioFormat is File, this must also be File. Otherwise, a choice of PCM encodings.
        /// </summary>
        public AudioFormatEncoding AudioFormatEncoding { get; internal set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="model"></param>
        /// <param name="sampleRate"></param>
        /// <param name="audioFormatType"></param>
        /// <param name="audioFormatEncoding"></param>
        public SmRtApiConfig(string model,
            int sampleRate,
            AudioFormatType audioFormatType,
            AudioFormatEncoding audioFormatEncoding)
        {
            if (audioFormatType == AudioFormatType.File && audioFormatEncoding != AudioFormatEncoding.File
                || audioFormatEncoding == AudioFormatEncoding.File && audioFormatType != AudioFormatType.File)
            {
                throw new ArgumentException("audioFormatType and audioFormatEncoding must both be File");
            }

            try
            {
                var culture = new CultureInfo(model);
            }
            catch(CultureNotFoundException ex)
            {
                throw new ArgumentException($"Invalid language code {model}", ex);
            }

            Model = model;
            SampleRate = sampleRate;
            AudioFormat = audioFormatType;
            AudioFormatEncoding = audioFormatEncoding;
        }

        /// <summary>
        /// Constructor for transcribing a file
        /// </summary>
        /// <param name="model"></param>
        public SmRtApiConfig(string model)
        {
            Model = model;
            SampleRate = 0;
            AudioFormat = AudioFormatType.File;
            AudioFormatEncoding = AudioFormatEncoding.File;
        }
    }
}