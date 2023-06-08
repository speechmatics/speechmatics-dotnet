using System.Collections.Generic;
using Speechmatics.Realtime.Client.Config;

namespace Speechmatics.Realtime.Client.Messages
{
    internal class SetRecognitionConfigMessage : BaseMessage
    {
        public string message => "SetRecognitionConfig";
        public Dictionary<string, object> transcription_config { get; }

        public SetRecognitionConfigMessage(SmRtApiConfig smConfig)
        {
            transcription_config = new Dictionary<string, object>();
            transcription_config["language"] = smConfig.Model;
            transcription_config["max_delay"] = smConfig.MaxDelay;
            transcription_config["enable_partials"] = smConfig.EnablePartials;

        }
    }
}
