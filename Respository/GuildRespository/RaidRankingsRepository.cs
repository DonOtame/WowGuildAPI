using MongoDB.Driver;
using WowGuildAPI.Models.GuildModels;
using WowGuildAPI.Respository.GuildRespository.Interfaces;
using WowGuildAPI.Services.Interfaces;
using WowGuildAPI.Utils;

namespace WowGuildAPI.Respository.GuildRespository
{
    public class RaidRankingsRespository : IRaidRankingsRepository
    {
        private readonly IMongoCollection<Guild> _guilds;
        private readonly IRaiderIoGuildProfileService _raiderIoService;
        private readonly IGuildRepository _guildRep;

        public RaidRankingsRespository(IMongoClient mongoClient, IRaiderIoGuildProfileService raiderIoService, IGuildRepository guildRep)
        {
            _guilds = mongoClient.GetDatabase("WowGuildDB").GetCollection<Guild>("guilds");
            _raiderIoService = raiderIoService;
            _guildRep = guildRep;
        }

        public async Task<Dictionary<string, RaidRanking>?> GetRaidRankingsAsync(string region, string realm, string name)
        {
            var guild = await _guildRep.GetGuildAsync(region, realm, name);
            if (guild == null) return null;

            var RaidRankings = await GetRaidRankingsFromDbAsync(region, realm, name);

            if (RaidRankings != null && RaidRankings.Count > 0 && !WowUtils.IsUpdateRequired(guild.LastUpdated))
            {
                return RaidRankings;
            }

            RaidRankings = await GetRaidRankingsFromApiAsync(region, realm, name);
            if (RaidRankings == null) return null;

            await SaveRaidRankingsToDbAsync(guild, RaidRankings);
            return RaidRankings;
        }

        private async Task<Dictionary<string, RaidRanking>?> GetRaidRankingsFromApiAsync(string region, string realm, string name)
        {
            try
            {
                return await _raiderIoService.GetRaidRankingsAsync(region, realm, name) is { Count: > 0 } RaidRankings ? RaidRankings : null;
            }
            catch
            {
                return null;
            }
        }

        private async Task<Dictionary<string, RaidRanking>?> GetRaidRankingsFromDbAsync(string region, string realm, string name)
        {
            var guildId = WowUtils.GenerateWowId(region, realm, name);
            return (await _guilds.Find(g => g.Id == guildId).FirstOrDefaultAsync())?.RaidRankings;
        }

        private async Task SaveRaidRankingsToDbAsync(Guild guild, Dictionary<string, RaidRanking> RaidRankings)
        {
            if (guild.RaidRankings?.SequenceEqual(RaidRankings) == true) return;

            guild.RaidRankings = RaidRankings;
            guild.LastUpdated = DateTime.UtcNow;
            await _guilds.ReplaceOneAsync(g => g.Id == guild.Id, guild, new ReplaceOptions { IsUpsert = true });
        }
    }
}
