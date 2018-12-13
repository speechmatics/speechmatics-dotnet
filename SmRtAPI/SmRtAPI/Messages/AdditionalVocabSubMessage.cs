using System.Collections.Generic;

namespace Speechmatics.Realtime.Client.Messages
{
    /* 
     * Looks like [ "foo", "bar", { "content" : "gnocchi", "sounds_like" : [ "nokey", "noki" ] } ]
     * */
    internal class AdditionalVocabSubMessage
    {
        public List<object> Data { get; }

        public AdditionalVocabSubMessage(IEnumerable<string> plainWords,
            IDictionary<string, IEnumerable<string>> soundsLikes)
        {
            Data = new List<object>();

            if (plainWords != null)
            {
                Data.AddRange(plainWords);
            }

            if (soundsLikes == null)
            {
                return;
            }

            foreach (var o in soundsLikes)
            {
                var t = new 
                {
                    content = o.Key,
                    sounds_like = o.Value
                };

                Data.Add(t);
            }
        }
    }
}