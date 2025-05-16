namespace Speechmatics.Realtime.Client.Enumerations
{
    /*
     *   * `model` (String): language product used to process the job (for example `en-US`)
  * `AudioFormatSubMessage` (Object:AudioFormatEncoding): audio stream type you the user is going to send: see [Supported audio types](#supported-audio-types).
  * `output_format` (Object:OutputFormat): Requested output format, see [Supported output formats](#supported-output-formats).

    */

    /// <summary>
    /// Type of audio
    /// </summary>
    public enum AudioFormatEncoding
    {
        /// <summary>
        /// PCM encoded 32-bit floating point (little-endian)
        /// </summary>
        PcmF32Le,
        /// <summary>
        /// PCM encoded 16-bit signed integer (little-endian)
        /// </summary>
        PcmS16Le,
        /// <summary>
        /// Audio file
        /// </summary>
        File,
        /// <summary>
        /// Mulaw - https://en.wikipedia.org/wiki/%CE%9C-law_algorithm
        /// </summary>
        Mulaw
    }
}
