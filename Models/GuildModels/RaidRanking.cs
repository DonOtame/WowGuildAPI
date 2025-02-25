using MongoDB.Bson.Serialization.Attributes;

namespace WowGuildAPI.Models.GuildModels
{
    public class RaidRanking
    {
        [BsonElement("normal")]
        public RaidLevel Normal { get; set; }

        [BsonElement("heroic")]
        public RaidLevel Heroic { get; set; }

        [BsonElement("mythic")]
        public RaidLevel Mythic { get; set; }
    }

    public class RaidLevel
    {
        [BsonElement("world")]
        public int World { get; set; }

        [BsonElement("region")]
        public int Region { get; set; }

        [BsonElement("realm")]
        public int Realm { get; set; }
    }
}
