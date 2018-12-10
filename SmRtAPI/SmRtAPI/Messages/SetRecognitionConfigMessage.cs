using System;
using System.Collections.Generic;
using System.Text;

namespace Speechmatics.Realtime.Client.Messages
{
    internal class SetRecognitionConfigMessage : BaseMessage
    {
        public SetRecognitionConfigMessage(AdditionalVocabSubMessage additionalVocab = null, 
            SpellingsRegionSubMessage spellingsRegion = null,
            DynamicTranscriptSubMessage dynamicTranscript = null
            )
        {
            config = new Dictionary<string, object>();
            if (additionalVocab != null)
            {
                additional_vocab = additionalVocab;
            }
            if (spellingsRegion != null)
            {
                config["spellings_region"] = spellingsRegion;
            }
            if (dynamicTranscript != null)
            {
                config["dynamic_transcript"] = dynamicTranscript;
            }
        }

        public override string message => "SetRecognitionConfig";
        public AdditionalVocabSubMessage additional_vocab { get; }
        public Dictionary<string, object> config { get; }
    }
}
