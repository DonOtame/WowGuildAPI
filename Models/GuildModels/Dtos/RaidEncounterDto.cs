namespace WowGuildAPI.Models.GuildModels.Dtos
{
    public class RaidEncounterDto
    {
        public string Slug { get; set; }
        public string Name { get; set; }
        public string BossImageUrl { get; set; }
        public Dictionary<string, DateTime> Defeats { get; set; }
    }


}
