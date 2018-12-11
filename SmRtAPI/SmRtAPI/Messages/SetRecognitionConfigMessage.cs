using System;
using System.Collections.Generic;
using System.Text;

namespace Speechmatics.Realtime.Client.Messages
{
    internal class SetRecognitionConfigMessage : BaseMessage
    {
        public SetRecognitionConfigMessage(AdditionalVocabSubMessage additionalVocab = null, 
            OutputLocaleSubMessage outputLocale = null,
            DynamicTranscriptSubMessage dynamicTranscript = null
            )
        {
            config = new Dictionary<string, object>();
            if (additionalVocab != null)
            {
                config["additional_vocab"] = additionalVocab.Data;
            }
            if (outputLocale != null)
            {
                config["output_locale"] = outputLocale;
            }
            if (dynamicTranscript != null)
            {
                config["dynamic_transcript"] = dynamicTranscript;
            }
        }

        public override string message => "SetRecognitionConfig";
        public Dictionary<string, object> config { get; }
    }
}
