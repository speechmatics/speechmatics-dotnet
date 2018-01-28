using NUnit.Framework;

namespace SpeechmaticsAPI.Tests
{
    [TestFixture]
    public class MessagingTests
    {
        [Test]
        public void Constructor()
        {
            var url = "wss://this/";
            var s = new SmRtApi(url);
            Assert.AreEqual(url, s.WsUrl, "Get WS url back");
        }
    }
}
