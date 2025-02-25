using WowGuildAPI.Models.CharacterModels;

namespace WowGuildAPI.Services.Interfaces
{
    public interface IRaiderIoCharacterProfileService
    {
        Task<Character?> GetCharacterAsync(string region, string realm, string name);
        Task<List<MythicPlusScores>> GetMythicPlusScoresAsync(string region, string realm, string name);
        Task<List<MythicPlusBestRuns>> GetMythicPlusBestRunsAsync(string region, string realm, string name);
    }
}
