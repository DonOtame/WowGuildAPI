using WowGuildAPI.Models.GuildModels;

namespace WowGuildAPI.Respository.GuildRespository.Interfaces
{
    public interface IRaidProgressionsRepository
    {
        Task<Dictionary<string, RaidProgression>?> GetRaidProgressionAsync(string region, string realm, string name);
    }
}
