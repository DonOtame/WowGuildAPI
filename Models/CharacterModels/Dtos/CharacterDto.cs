using MongoDB.Bson.Serialization.Attributes;

namespace WowGuildAPI.Models.CharacterModels.Dtos
{
    public class CharacterDto
    {
        public string Name { get; set; }
        public string Race { get; set; }
        public string Class { get; set; }
        public string ActiveSpecName { get; set; }
        public string ActiveSpecRole { get; set; }
        public string Faction { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Region { get; set; }
        public string Realm { get; set; }
        public string ProfileUrl { get; set; }

    }
}
