using WowGuildAPI.Models.GuildModels;

namespace WowGuildAPI.Services.Interfaces
{
    public interface IRaiderIoGuildProfileService
    {
        Task<Guild?> GetGuildAsync(string region, string realm, string name);
        Task<Dictionary<string, RaidProgression>> GetRaidProgressionsAsync(string region, string realm, string name);
        Task<Dictionary<string, RaidRanking>> GetRaidRankingsAsync(string region, string realm, string name);
        Task<List<Member>> GetMembersAsync(string region, string realm, string name);
    }
}
