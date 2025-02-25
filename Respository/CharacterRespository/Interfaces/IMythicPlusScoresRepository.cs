using WowGuildAPI.Models.CharacterModels;

namespace WowGuildAPI.Respository.CharacterRespository.Interfaces
{
    public interface IMythicPlusScoresRepository
    {
        Task<List<MythicPlusScores>?> GetMythicPlusScoresAsync(string region, string realm, string name);
    }
}
