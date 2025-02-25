using MongoDB.Bson.Serialization.Attributes;

namespace WowGuildAPI.Models.GuildModels
{
    public class Member
    {
        [BsonElement("rank")]
        public int Rank { get; set; }

        [BsonElement("character")]
        public Character Character { get; set; }
    }

    public class Character
    {
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("region")]
        public string Region { get; set; }

        [BsonElement("realm")]
        public string Realm { get; set; }
    }

}