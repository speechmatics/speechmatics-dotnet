namespace Speechmatics.Realtime.Client.Messages
{
    internal class OutputLocaleSubMessage
    {
        public OutputLocaleSubMessage(string outputLocale)
        {
            output_locale = outputLocale;
        }

        public string output_locale { get; }
    }
}