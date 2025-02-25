namespace WowGuildAPI.Models.GuildModels.Dtos
{
    public class MemberDto
    {
        public int Rank { get; set; }
        public CharacterDto Character { get; set; }
    }

    public class CharacterDto
    {
        public string Name { get; set; }
        public string Region { get; set; }
        public string Realm { get; set; }
    }
}
