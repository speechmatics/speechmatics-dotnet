using System;
using Newtonsoft.Json;

namespace Speechmatics.Realtime.Client.Messages
{
    /*
     *   * `message: "Error"`
  * `code` (Int): A numerical code for the error. See below. TODO: This is not yet finalised.
  * `type` (String): A code for the error message. See the list of possible errors below.
  * `reason` (String): A human-readable reason for the error message.
  * `seq_no` (Int, optional): A `seq_no` of a corresponding API call, only present if the `seq_no` was specified in the API call.
  * */

    /// <summary>
    /// Message returned from server in the case of warning
    /// </summary>
    public class WarningMessage : BaseMessage
    {
        /// <summary>
        /// Human-readable reason
        /// </summary>
        public string reason = String.Empty;
        /// <summary>
        /// Message type
        /// </summary>
        public string message => "Warning";
        /// <summary>
        /// Code for the warning
        /// </summary>
        public string type = String.Empty;
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