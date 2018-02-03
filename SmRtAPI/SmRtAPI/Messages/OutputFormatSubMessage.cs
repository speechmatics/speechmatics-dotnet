using SpeechmaticsAPI.Enumerations;

namespace SpeechmaticsAPI.Messages
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