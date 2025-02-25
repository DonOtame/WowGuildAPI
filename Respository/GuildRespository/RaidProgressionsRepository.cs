
using MongoDB.Driver;
using WowGuildAPI.Models.GuildModels;
using WowGuildAPI.Respository.GuildRespository.Interfaces;
using WowGuildAPI.Services;
using WowGuildAPI.Services.Interfaces;
using WowGuildAPI.Utils;

namespace WowGuildAPI.Respository.GuildRespository
{
    public class RaidProgressionsRepository : IRaidProgressionsRepository
    {
        private readonly IMongoCollection<Guild> _guilds;
        private readonly IRaiderIoGuildProfileService _raiderIoService;
        private readonly IGuildRepository _guildRep;

        public RaidProgressionsRepository(IMongoClient mongoClient, IRaiderIoGuildProfileService raiderIoService, IGuildRepository guildRep)
        {
            _guilds = mongoClient.GetDatabase("WowGuildDB").GetCollection<Guild>("guilds");
            _raiderIoService = raiderIoService;
            _guildRep = guildRep;
        }

        public async Task<Dictionary<string, RaidProgression>?> GetRaidProgressionAsync(string region, string realm, string name)
        {
            var guild = await _guildRep.GetGuildAsync(region, realm, name);
            if (guild == null) return null;

            var raidProgressions = await GetRaidProgressionsFromDbAsync(region, realm, name);

            if (raidProgressions != null && raidProgressions.Count > 0 && !WowUtils.IsUpdateRequired(guild.LastUpdated))
            {
                return raidProgressions;
            }

            raidProgressions = await GetRaidProgressionFromApiAsync(region, realm, name);
            if (raidProgressions == null) return null;

            await SaveRaidProgressionsToDbAsync(guild, raidProgressions);
            return raidProgressions;
        }

        private async Task<Dictionary<string, RaidProgression>?> GetRaidProgressionFromApiAsync(string region, string realm, string name)
        {
            try
            {
                return await _raiderIoService.GetRaidProgressionsAsync(region, realm, name) is { Count: > 0 } raidProgressions ? raidProgressions : null;
            }
            catch
            {
                return null;
            }
        }

        private async Task<Dictionary<string, RaidProgression>?> GetRaidProgressionsFromDbAsync(string region, string realm, string name)
        {
            var guildId = WowUtils.GenerateWowId(region, realm, name);
            return (await _guilds.Find(g => g.Id == guildId).FirstOrDefaultAsync())?.RaidProgressions;
        }

        private async Task SaveRaidProgressionsToDbAsync(Guild guild, Dictionary<string, RaidProgression> raidProgressions)
        {
            if (guild.RaidProgressions?.SequenceEqual(raidProgressions) == true) return;

            guild.RaidProgressions = raidProgressions;
            guild.LastUpdated = DateTime.UtcNow;
            await _guilds.ReplaceOneAsync(g => g.Id == guild.Id, guild, new ReplaceOptions { IsUpsert = true });
        }
    }
}
