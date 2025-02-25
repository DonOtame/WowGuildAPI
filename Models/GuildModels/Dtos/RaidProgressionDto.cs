namespace WowGuildAPI.Models.GuildModels.Dtos
{
    public class RaidProgressionDto
    {
        public string Summary { get; set; }
        public string BackgroundImageUrl { get; set; }
        public int TotalBosses { get; set; }
        public BossKilledDto BossesKilled { get; set; }
        public Dictionary<string, RaidEncounterDto> RaidEncounters { get; set; }


    }
    public class BossKilledDto
    {
        public int Normal { get; set; }
        public int Heroic { get; set; }
        public int Mythic { get; set; }
    }
}
