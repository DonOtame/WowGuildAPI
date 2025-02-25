using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace WowGuildAPI.Models.GuildModels
{
    public class Guild
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("faction")]
        public string Faction { get; set; } = string.Empty;

        [BsonElement("region")]
        public string Region { get; set; } = string.Empty;

        [BsonElement("realm")]
        public string Realm { get; set; } = string.Empty;

        [BsonElement("last_crawled_at")]
        public DateTime LastCrawledAt { get; set; }

        [BsonElement("profile_url")]
        public string ProfileUrl { get; set; } = string.Empty;
        [BsonElement("raid_progressions")]
        public Dictionary<string, RaidProgression>? RaidProgressions { get; set; }
        [BsonElement("raid_rankings")]
        public Dictionary<string, RaidRanking>? RaidRankings { get; set; }
        [BsonElement("members")]
        public List<Member>? Members { get; set; }

        [BsonElement("last_updated")]
        public DateTime LastUpdated { get; set; }
    }

}
