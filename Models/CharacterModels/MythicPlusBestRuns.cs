using MongoDB.Bson.Serialization.Attributes;

namespace WowGuildAPI.Models.CharacterModels
{
    public class MythicPlusBestRuns
    {
        [BsonElement("dungeon")]
        public string Dungeon { get; set; }

        [BsonElement("short_name")]
        public string ShortName { get; set; }

        [BsonElement("mythic_level")]
        public int MythicLevel { get; set; }

        [BsonElement("completed_at")]
        public DateTime CompletedAt { get; set; }

        [BsonElement("clear_time_ms")]
        public double ClearTimeMs { get; set; }

        [BsonElement("keystone_run_id")]
        public int KeystoneRunId { get; set; }

        [BsonElement("par_time_ms")]
        public double ParTimeMs { get; set; }

        [BsonElement("num_keystone_upgrades")]
        public int NumKeystoneUpgrades { get; set; }

        [BsonElement("map_challenge_mode_id")]
        public int MapChallengeModeId { get; set; }

        [BsonElement("zone_id")]
        public int ZoneId { get; set; }

        [BsonElement("zone_expansion_id")]
        public int ZoneExpansionId { get; set; }

        [BsonElement("icon_url")]
        public string IconUrl { get; set; }

        [BsonElement("background_image_url")]
        public string BackgroundImageUrl { get; set; }

        [BsonElement("score")]
        public double Score { get; set; }

        [BsonElement("url")]
        public string Url { get; set; }

        [BsonElement("affixes")]
        public List<Affix> Affixes { get; set; }
    }

    public class Affix
    {
        [BsonElement("id")]
        public int Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("icon_url")]
        public string IconUrl { get; set; }
    }
}
