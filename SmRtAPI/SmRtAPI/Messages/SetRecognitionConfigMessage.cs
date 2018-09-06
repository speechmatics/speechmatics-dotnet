using System;
using System.Collections.Generic;
using System.Text;

namespace Speechmatics.Realtime.Client.Messages
{
    internal class SetRecognitionConfigMessage : BaseMessage
    {
        public SetRecognitionConfigMessage(AdditionalVocabSubMessage additionalVocab)
        {
            config = additionalVocab;
        }

        public override string message => "SetRecognitionConfig";
        public AdditionalVocabSubMessage config { get; }
    }
}
