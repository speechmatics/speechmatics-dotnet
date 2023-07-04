namespace Speechmatics.Realtime.Client.Messages
{

    public class TranslationSubMessage: BaseMessage 
    {
        /// <summary>
        ///  The start time (in seconds) of the original transcribed audio segment.
        /// </summary>
        public double start_time;

        /// <summary>
        /// The end time (in seconds) of the original transcribed audio segment.
        /// </summary>
        public double end_time; 

        /// <summary>
        /// The translated segment of speech
        /// </summary>
        public string content;

        /// <summary>
        /// The speaker that uttered the speech if speaker diarization is enabled. See Transcription config.
        /// </summary>
        public string? speaker;
    }

    /// <summary>
    ///  Each message corresponds to the audio since the last AddTranslation message. 
    /// These messages are also referred to as Finals since the transcript will not change any further. 
    /// An AddTranslation message is sent when we reach the end of a sentence in the transcription. 
    /// Any further AddTranslation or Partial messages will only correspond to the newly processed audio.
    /// </summary>
    public class AddTranslationMessage : BaseMessage
    {
        /// <summary>
        /// Message type
        /// </summary>
        public string message => "AddTranslation";

        /// <summary>
        /// Target language translation relates to
        /// </summary>
        public string language;

        /// <summary>
        /// List of translated sentences.
        /// </summary>
        public TranslationSubMessage[] results;
    }
}