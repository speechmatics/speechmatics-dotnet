namespace SpeechmaticsAPI
{
    public class SmRtApi
    {
        public string WsUrl { get; }

        public SmRtApi(string wsUrl)
        {
            WsUrl = wsUrl;
        }
    }
}
