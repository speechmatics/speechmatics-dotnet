using System.Collections.Generic;
using Speechmatics.Realtime.Client.Messages;
using Speechmatics.Realtime.Client.V2.Config;

namespace Speechmatics.Realtime.Client.V2.Messages
{
    internal class SetRecognitionConfigMessage : BaseMessage
    {
        public string message => "SetRecognitionConfig";
        public Dictionary<string, object> transcription_config { get; }

        public SetRecognitionConfigMessage(SmRtApiConfig smConfig, AdditionalVocabSubMessage additionalVocab = null)
        {
            transcription_config = new Dictionary<string, object>();
            transcription_config["language"] = "en";
            if (additionalVocab != null)
            {
                transcription_config["additional_vocab"] = additionalVocab.Data;
            }
            if (smConfig.OutputLocale != null)
            {
                transcription_config["output_locale"] = smConfig.OutputLocale;
            }

            transcription_config["max_delay"] = smConfig.MaxDelay;
            transcription_config["enable_partials"] = smConfig.EnablePartials;

        }
    }
}
