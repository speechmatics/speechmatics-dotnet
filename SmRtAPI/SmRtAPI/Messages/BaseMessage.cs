using System.IO;
using Newtonsoft.Json;

namespace SpeechmaticsAPI.Messages
{
    public abstract class BaseMessage
    {
        public string AsJson()
        {
            using (var sw = new StringWriter())
            {
                JsonSerializer.Create().Serialize(sw, this);
                return sw.ToString();
            }
        }

        public abstract string message { get; }
    }
}