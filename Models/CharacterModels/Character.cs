using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace WowGuildAPI.Models.CharacterModels
{
    public class Character
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("race")]
        public string Race { get; set; }

        [BsonElement("class")]
        public string Class { get; set; }

        [BsonElement("active_spec_name")]
        public string ActiveSpecName { get; set; }

        [BsonElement("active_spec_role")]
        public string ActiveSpecRole { get; set; }

        [BsonElement("faction")]
        public string Faction { get; set; }

        [BsonElement("thumbnail_url")]
        public string ThumbnailUrl { get; set; }

        [BsonElement("region")]
        public string Region { get; set; }

        [BsonElement("realm")]
        public string Realm { get; set; }

        [BsonElement("last_crawled_at")]
        public DateTime LastCrawledAt { get; set; }

        [BsonElement("profile_url")]
        public string ProfileUrl { get; set; }

        [BsonElement("mythic_plus_scores")]
        public List<MythicPlusScores>? MythicPlusScores { get; set; }

        [BsonElement("mythic_plus_runs")]
        public List<MythicPlusBestRuns>? MythicPlusBestRuns { get; set; }

        [BsonElement("last_updated")]
        public DateTime LastUpdated { get; set; }
    }
}
