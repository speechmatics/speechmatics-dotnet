using Newtonsoft.Json;

namespace Speechmatics.Realtime.Client.Messages
{
    public class ErrorMessage : BaseMessage
    {
        public string reason;
        public override string message => "Error";
        public string type;
        [JsonProperty(Required = Required.Default)]
        public int seq_no;
        public float duration_limit;
    }
}