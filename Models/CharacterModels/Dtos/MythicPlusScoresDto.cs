namespace WowGuildAPI.Models.CharacterModels.Dtos
{
    public class MythicPlusScoresDto
    {
        public string Season { get; set; } = string.Empty;
        public bool IsCurrentSeason { get; set; }
        public Dictionary<string, SegmentsDto> Segments { get; set; } = new();
    }

    public class SegmentsDto
    {
        public double Score { get; set; }
        public string Color { get; set; } = "#ffffff";
    }
}
