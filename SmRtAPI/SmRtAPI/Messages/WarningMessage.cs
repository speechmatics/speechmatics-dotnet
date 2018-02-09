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

    public class WarningMessage : BaseMessage
    {
        public string reason;
        public override string message => "Warning";
        public string type;
        [JsonProperty(Required = Required.Default)]
        public int seq_no;
        public float duration_limit;
    }
}