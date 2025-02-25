using WowGuildAPI.Models.GuildModels;

namespace WowGuildAPI.Respository.GuildRespository.Interfaces
{
    public interface IGuildRepository
    {
        Task<Guild?> GetGuildAsync(string region, string realm, string name);
    }

}
