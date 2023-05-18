using Speechmatics.Realtime.Client.Messages;
using System.Collections.Generic;

namespace Speechmatics.Realtime.Client.V2.Messages
{
    internal class StartRecognitionMessage : BaseMessage
    {
        public StartRecognitionMessage(AudioFormatSubMessage audioFormatSubMessage, SmRtApiConfigBase smConfig)
        {
            audio_format = audioFormatSubMessage;
            transcription_config["language"] = smConfig.Model;

            transcription_config["max_delay"] = smConfig.MaxDelay;
            transcription_config["enable_partials"] = smConfig.EnablePartials;
        }

        public override string message => "StartRecognition";
        public AudioFormatSubMessage audio_format { get; }
        public Dictionary<string, object> transcription_config { get; } = new Dictionary<string, object>();
    }
}