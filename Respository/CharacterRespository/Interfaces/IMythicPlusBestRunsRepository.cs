using WowGuildAPI.Models.CharacterModels;

namespace WowGuildAPI.Respository.CharacterRespository.Interfaces
{
    public interface IMythicPlusBestRunsRepository
    {
        Task<List<MythicPlusBestRuns>?> GetMythicPlusBestRunsAsync(string region, string realm, string name);
    }
}
