namespace WowGuildAPI.Models.CharacterModels.Dtos
{
    public class MythicPlusBestRunsDto
    {
        public string Dungeon { get; set; }
        public string ShortName { get; set; }
        public int MythicLevel { get; set; }
        public DateTime CompletedAt { get; set; }
        public double ClearTimeMs { get; set; }
        public int KeystoneRunId { get; set; }
        public double ParTimeMs { get; set; }
        public int NumKeystoneUpgrades { get; set; }
        public int MapChallengeModeId { get; set; }
        public int ZoneId { get; set; }
        public int ZoneExpansionId { get; set; }
        public string IconUrl { get; set; }
        public string BackgroundImageUrl { get; set; }
        public double Score { get; set; }
        public string Url { get; set; }
        public List<AffixDto> Affixes { get; set; }
    }
    public class AffixDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string IconUrl { get; set; }
    }
}
