namespace Speechmatics.Realtime.Client.Enumerations
{
    /*
     *   * `model` (String): language product used to process the job (for example `en-US`)
  * `AudioFormatSubMessage` (Object:AudioFormatEncoding): audio stream type you the user is going to send: see [Supported audio types](#supported-audio-types).
  * `output_format` (Object:OutputFormat): Requested output format, see [Supported output formats](#supported-output-formats).

    */

    public enum AudioFormatEncoding
    {
        PcmF32Le,
        PcmS16Le,
        File
    }
}
