using System.Collections.Generic;
using System.Linq;

namespace Speechmatics.Realtime.Client.Messages
{
    /* 
     * Looks like [ "foo", "bar", { "content" : "gnocchi", "sounds_like" : [ "nokey", "noki" ] } ]
     * */
    internal class AdditionalVocabSubMessage
    {
        class SoundsLike
        {
            public string content { get; set; }
            public IEnumerable<string> sounds_like { get; set; }
        }

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
                var t = new SoundsLike
                {
                    content = o.Key,
                    sounds_like = o.Value
                };

                Data.Add(t);
            }
        }
    }
}