namespace Speechmatics.Realtime.Client.Enumerations
{
    /// <summary>
    /// Type of audio, raw, file..
    /// </summary>
    public enum AudioFormatType
    {
        /// <summary>
        /// Audio is raw audio samples, e.g. from a microphone
        /// </summary>
        Raw,
        /// <summary>
        /// Audio is a file stream
        /// </summary>
        File,
    }
}