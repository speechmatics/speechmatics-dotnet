using System;
using System.Collections.Generic;
using System.Globalization;
using Speechmatics.Realtime.Client.Enumerations;
using Speechmatics.Realtime.Client.Messages;
using Speechmatics.Realtime.Client.Config;

namespace Speechmatics.Realtime.Client
{
    /// <summary>
    /// Configuration for an SmRtApi session
    /// </summary>
    public class SmRtApiConfigBase
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
        /// Action to perform on receiving a transcript
        /// </summary>
        public Action<string> AddTranscriptCallback { get; set; }

        /// <summary>
        /// Action to perform on receiving a partial
        /// </summary>
        public Action<string> AddPartialTranscriptCallback { get; set; }
        /// <summary>
        /// Action to perform on end of transcript
        /// </summary>
        public Action EndOfTranscriptCallback { get; set; }
        /// <summary>
        /// Action to perform when a warning message is received
        /// </summary>
        public Action<Messages.WarningMessage> WarningMessageCallback { get; set; }
        /// <summary>
        /// Action to perform when an error message is received
        /// </summary>
        public Action<Messages.ErrorMessage> ErrorMessageCallback { get; set; }
        /// <summary>
        /// True if SSL errors should be ignored (default false)
        /// </summary>
        public bool Insecure { get; set; }
        /// <summary>
        /// A list of words to add to the custom dictionary. Pronunciation will be inferred.
        /// </summary>
        public IEnumerable<string> CustomDictionaryPlainWords { get; set; }
        /// <summary>
        /// A mapping of words to alternative phonetic pronunciations, e.g. "gnocchi" => ("nokey", "noki")
        /// </summary>
        public IDictionary<string, IEnumerable<string>> CustomDictionarySoundsLikes { get; set; }
        /// <summary>
        /// For language models which support it, an optional output locale to use for output. e.g. en-GB, en-US (default), en-AU.
        /// </summary>
        public string OutputLocale { get; set; }
        /// <summary>
        /// Data block size to send in one message. Overly small or large values can overload the server, something like 8192
        /// is usually safe. Large block sizes can affect latency.
        /// </summary>
        public int BlockSize { get; set; }

        /// <summary>
        /// API Authentication Token
        /// only applicable for RT SaaS customers.
        /// </summary>
        public string AuthToken { get; set; }

        /// <summary>
        /// API keys obtained through portal.speechmatics.com
        /// need to set this to true
        /// </summary>
        public bool GenerateTempToken { get; set; } = false;

        /// <summary>
        /// Internal Speechmatics flag that allows to give special commands to the engine.
        /// </summary>
        public string Ctrl { get; set; }

        /// <summary>
        /// Maximum acceptable delay in seconds
        /// Forces a "final" transcription every x seconds
        /// </summary>
        public double MaxDelay { get; set; } = 5;

        /// <summary>
        /// Determines whether the threshold specified in max_delay can be exceeded
        /// if a potential entity is detected.Flexible means if a potential entity
        /// is detected, then the max_delay can be overriden until the end of that
        /// entity. Fixed means that max_delay specified ignores any potential
        /// entity that would not be completed within that threshold
        /// </summary>
        public string? MaxDelayMode { get; set; } = null;

        /// <summary>
        /// Indicates if partials for both transcripts and translation,
        /// where words are produced immediately, is enabled
        /// </summary>
        public bool EnablePartials { get; set; } = false;

        /// <summary>
        /// Indicates if we run the engine in streaming mode, or regular RT mode.
        /// </summary>
        public bool StreamingMode { get; set; } = false;

        /// <summary>
        /// Optional. Whether a user wishes for entities to be identified with additional spoken and written word format
        /// </summary>
        public bool EnableEntities { get; set; } = false;


        /// <summary>
        /// Which operating point to use for transcription.
        /// Valid values are "standard" and "enhanced"
        /// See https://docs.speechmatics.com/features/accuracy-language-packs#accuracy for the difference between them
        /// </summary>
        public string? OperatingPoint { get; set; } = null;

        /// <summary>
        /// </summary>
        public DiarizationType Diarization { get; set; } = DiarizationType.None;

        /// <summary>
        /// Extra configuration for Speaker Diarization
        /// </summary>
        public SpeakerDiarizationConfig? SpeakerDiarizationConfig { get; set; }

        /// <summary>
        /// Optional configuration for customizing punctuation
        /// </summary>
        public PunctuationConfig? PunctuationOverrides { get; set; }

        /// <summary>
        /// Optionally request a language pack optimized for a specific domain, e.g. 'finance'"
        /// </summary>
        public string? Domain { get; set; }

        public TranslationConfig? TranslationConfig { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="model"></param>
        /// <param name="sampleRate"></param>
        /// <param name="audioFormatType"></param>
        /// <param name="audioFormatEncoding"></param>
        public SmRtApiConfigBase(string model,
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
                var unused = new CultureInfo(model);
            }
            catch (CultureNotFoundException ex)
            {
                throw new ArgumentException($"Invalid language code {model}", ex);
            }

            Model = model;
            SampleRate = sampleRate;
            AudioFormat = audioFormatType;
            AudioFormatEncoding = audioFormatEncoding;
            BlockSize = 2048;
        }

        /// <summary>
        /// Constructor for transcribing a file
        /// </summary>
        /// <param name="model"></param>
        public SmRtApiConfigBase(string model)
        {
            Model = model;
            SampleRate = 0;
            AudioFormat = AudioFormatType.File;
            AudioFormatEncoding = AudioFormatEncoding.File;
            BlockSize = 2048;
        }
    }
}
