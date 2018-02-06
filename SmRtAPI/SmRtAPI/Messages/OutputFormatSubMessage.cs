using Speechmatics.Realtime.Client.Enumerations;

namespace Speechmatics.Realtime.Client.Messages
{
    public class OutputFormatSubMessage
    {
        public OutputFormatSubMessage(OutputFormat outputFormat)
        {
            type = outputFormat.ToApiString();
        }

        public string type { get; }
    }
}