using WowGuildAPI.Models.GuildModels;

namespace WowGuildAPI.Respository.GuildRespository.Interfaces
{
    public interface IRaidRankingsRepository
    {
        Task<Dictionary<string, RaidRanking>?> GetRaidRankingsAsync(string region, string realm, string name);
    }
}
