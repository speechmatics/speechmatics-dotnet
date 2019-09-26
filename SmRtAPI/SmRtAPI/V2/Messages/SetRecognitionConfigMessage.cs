using System.Collections.Generic;
using Speechmatics.Realtime.Client.Messages;

namespace Speechmatics.Realtime.Client.V2.Messages
{
    internal class SetRecognitionConfigMessage : BaseMessage
    {
        public override string message => "SetRecognitionConfig";
        public Dictionary<string, object> transcription_config { get; }

        public SetRecognitionConfigMessage(AdditionalVocabSubMessage additionalVocab = null, 
            string outputLocale = null,
            DynamicTranscriptConfiguration dynamicTranscript = null
            )
        {
            transcription_config = new Dictionary<string, object>();
            transcription_config["language"] = "en";
            if (additionalVocab != null)
            {
                transcription_config["additional_vocab"] = additionalVocab.Data;
            }
            if (outputLocale != null)
            {
                transcription_config["output_locale"] = outputLocale;
            }
            if (dynamicTranscript != null)
            {
                if (dynamicTranscript.UseDefaults)
                {
                    transcription_config["dynamic_transcript"] = new
                    {
                        enabled = dynamicTranscript.Enabled
                    };
                }
                else
                {
                    transcription_config["dynamic_transcript"] = new
                    {
                        max_chars = dynamicTranscript.MaxChars,
                        min_context = dynamicTranscript.MinContext,
                        max_delay = dynamicTranscript.MaxDelay,
                        enabled = dynamicTranscript.Enabled
                    };
                }
            }
        }
    }
}
