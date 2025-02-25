using MongoDB.Bson.Serialization.Attributes;

namespace WowGuildAPI.Models.GuildModels
{
    public class RaidProgression
    {
        [BsonElement("summary")]
        public string Summary { get; set; }
        [BsonElement("background_image_url")]
        public string BackgroundImageUrl { get; set; }
        [BsonElement("total_bosses")]
        public int TotalBosses { get; set; }
        [BsonElement("bosses_killed")]
        public BossKilled BossesKilled { get; set; }
        [BsonElement("bosses_encounters")]
        public Dictionary<string,RaidEncounter> RaidEncounters { get; set; }
    }

    public class BossKilled
    {
        [BsonElement("normal")]
        public int Normal { get; set; }
        [BsonElement("heroic")]
        public int Heroic { get; set; }
        [BsonElement("mythic")]
        public int Mythic { get; set; }
    }
}
