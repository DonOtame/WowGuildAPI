using MongoDB.Driver;
using WowGuildAPI.Models.GuildModels;
using WowGuildAPI.Respository.GuildRespository.Interfaces;
using WowGuildAPI.Services;
using WowGuildAPI.Services.Interfaces;
using WowGuildAPI.Utils;

namespace WowGuildAPI.Respository.GuildRespository
{
    public class MembersRespository : IMembersRepository
    {
        private readonly IMongoCollection<Guild> _guilds;
        private readonly IRaiderIoGuildProfileService _raiderIoService;
        private readonly IGuildRepository _guildRep;

        public MembersRespository(IMongoClient mongoClient, IRaiderIoGuildProfileService raiderIoService, IGuildRepository guildRep)
        {
            _guilds = mongoClient.GetDatabase("WowGuildDB").GetCollection<Guild>("guilds");
            _raiderIoService = raiderIoService;
            _guildRep = guildRep;
        }

        public async Task<List<Member>?> GetMembersAsync(string region, string realm, string name)
        {
            var guild = await _guildRep.GetGuildAsync(region, realm, name);
            if (guild == null) return null;

            var members = await GetMembersFromDbAsync(region, realm, name);

            if (members != null && members.Count > 0 && !WowUtils.IsUpdateRequired(guild.LastUpdated))
            {
                return members;
            }

            members = await GetMembersFromApiAsync(region, realm, name);
            if (members == null) return null;

            await SaveMembersToDbAsync(guild, members);
            return members;
        }

        private async Task<List<Member>?> GetMembersFromApiAsync(string region, string realm, string name)
        {
            try
            {
                return await _raiderIoService.GetMembersAsync(region, realm, name) is { Count: > 0 } members ? members : null;
            }
            catch
            {
                return null;
            }
        }

        private async Task<List<Member>?> GetMembersFromDbAsync(string region, string realm, string name)
        {
            var guildId = WowUtils.GenerateWowId(region, realm, name);
            return (await _guilds.Find(g => g.Id == guildId).FirstOrDefaultAsync())?.Members;
        }

        private async Task SaveMembersToDbAsync(Guild guild, List<Member> members)
        {
            if (guild.Members?.SequenceEqual(members) == true) return;

            guild.Members = members;
            guild.LastUpdated = DateTime.UtcNow;
            await _guilds.ReplaceOneAsync(g => g.Id == guild.Id, guild, new ReplaceOptions { IsUpsert = true });
        }
    }
}
