namespace WowGuildAPI.Models.GuildModels.Dtos
{
    public class RaidRankingDto
    {
        public RaidLevelDto Normal { get; set; }
        public RaidLevelDto Heroic { get; set; }
        public RaidLevelDto Mythic { get; set; }
    }

    public class RaidLevelDto
    {
        public int World { get; set; }
        public int Region { get; set; }
        public int Realm { get; set; }
    }

}
