using MongoDB.Bson.Serialization.Attributes;

namespace WowGuildAPI.Models.GuildModels
{
    public class RaidEncounter
    {
        [BsonElement("slug")]
        public string Slug { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("boss_Image_Url")]
        public string BossImageUrl { get; set; }

        [BsonElement("defeats")]
        public Dictionary<string, DateTime> Defeats { get; set; }

    }
}
