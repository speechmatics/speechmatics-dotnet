using Speechmatics.Realtime.Client.Enumerations;

namespace Speechmatics.Realtime.Client.Messages
{
    /// <summary>
    /// Output format
    /// </summary>
    public class OutputFormatSubMessage
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="outputFormat"></param>
        public OutputFormatSubMessage(OutputFormat outputFormat)
        {
            type = outputFormat.ToApiString();
        }

        /// <summary>
        /// Text representation of an OutputFormat
        /// </summary>
        public string type { get; }
    }
}