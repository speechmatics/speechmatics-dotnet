namespace Speechmatics.Realtime.Client.Messages
{
    internal class SpellingsRegionSubMessage
    {
        public SpellingsRegionSubMessage(string spellingsRegion)
        {
            spellings_region = spellingsRegion;
        }

        public string spellings_region { get; }
    }
}