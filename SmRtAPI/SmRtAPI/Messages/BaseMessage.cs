using System;
using System.Diagnostics;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SpeechmaticsAPI.Messages
{
    public class WordSubMessage : BaseMessage
    {
        public override string message => "Word";
        public string word;
        public double start_time;
        public double length;
    }

    public class AddTranscriptMessage : BaseMessage
    {
        /*
         *   * `message: "AddTranscript"`
  * `start_time` (Number): The start time of the audio (in seconds) that the **transcript** covers. It does not have to correspond to the first entry in the transcript itself, in case there is simply no transcript for the initial part of the time this transcript covers - this time includes any silence and is not meant to be treated as a word alignment.
  * `length` (Number): The length of the audio (of seconds) that the **transcript** covers. It includes any optional silence as well - it corresponds to the length of the raw audio processed to get this transcript.
  * `transcript` (String/Object): The partial transcript of the audio, in a string format. For a JSON output format, the value will be the JSON object directly. For any other type, it will be a string. *Currently, it's always set to a string.*
    * String: a few examples to say it all - "Hi. ", "This is an example. "
  * `words` (An array of Word objects) [Added in v0.2.0] - contains all individual word in the `transcript` sorted by their time. A special `<sb>` word indicates a sentence boundary, which usually means a full stop in `transcript`. A `Word` object has the following properties:
    * `word` (String): A lowercase representation of a word. In the future, this will have the same case as the same word appearing in `transcript`.
    * `start_time` (Number): An approximate time of occurence (in seconds) of the audio corresponding to the beginning of the word.
    * `length` (Number) [Added in 0.5.0]: An approximate duration (in seconds) of the audio correpsonding to the word.
         * 
         */

        public override string message => "AddTranscript";
        public double start_time;
        public double length;
        public string transcript;
        public WordSubMessage[] words;
    }

    public abstract class BaseMessage
    {
        public string AsJson()
        {
            using (var sw = new StringWriter())
            {
                JsonSerializer.Create().Serialize(sw, this);
                return sw.ToString();
            }
        }

        public async Task Send(ClientWebSocket webSocket, CancellationToken token)
        {
            var asJson = AsJson();

            var bytes = Encoding.UTF8.GetBytes(asJson);
            await webSocket.SendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length), WebSocketMessageType.Text, true,
                token).ContinueWith(t =>
            {
                Debug.WriteLine("Sent {0} {1}, faulted={2}, status={3}", message, asJson, t.IsFaulted, t.Status);
            }, token);
        }

        public abstract string message { get; }
    }
}