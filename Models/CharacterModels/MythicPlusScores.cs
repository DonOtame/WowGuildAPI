using MongoDB.Bson.Serialization.Attributes;

namespace WowGuildAPI.Models.CharacterModels
{
    public class MythicPlusScores
    {

        [BsonElement("season")]
        public string Season { get; set; } = string.Empty;

        [BsonElement("is_current_season")]
        public bool IsCurrentSeason { get; set; }

        [BsonElement("segments")]
        public Dictionary<string, Segments> Segments { get; set; } = new();

    }

    public class Segments
    {
        [BsonElement("score")]
        public double Score { get; set; }

        [BsonElement("color")]
        public string Color { get; set; } = "#ffffff";
    }

}
