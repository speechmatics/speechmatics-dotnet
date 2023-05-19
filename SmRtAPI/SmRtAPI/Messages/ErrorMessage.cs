using Newtonsoft.Json;

namespace Speechmatics.Realtime.Client.Messages
{
    /// <summary>
    /// Message returned from server in the case of error
    /// </summary>
    public class ErrorMessage : BaseMessage
    {
        /// <summary>
        /// Reason
        /// </summary>
        public string reason;
        /// <summary>
        /// Message type
        /// </summary>
        public string message => "Error";
        /// <summary>
        /// Code for the warning
        /// </summary>
        public string type;
        /// <summary>
        /// Seq no of corresponding API call which caused the error
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public int seq_no;
        /// <summary>
        /// RT can be configured to limit the stream length
        /// </summary>
        public float duration_limit;
    }
}