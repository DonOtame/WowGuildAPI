using WowGuildAPI.Models.GuildModels;

namespace WowGuildAPI.Respository.GuildRespository.Interfaces
{
    public interface IMembersRepository
    {
        Task<List<Member>?> GetMembersAsync(string region, string realm, string name);
    }
}
