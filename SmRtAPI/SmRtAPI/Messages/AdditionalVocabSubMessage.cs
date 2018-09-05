namespace Speechmatics.Realtime.Client.Messages
{
    internal class AdditionalVocabSubMessage
    {
        public string[] additional_vocab { get; set; }

        public AdditionalVocabSubMessage()
        {
            additional_vocab = new string[] {"foo", "bar"};
        }
    }
}