using MongoDB.Driver;
using WowGuildAPI.Models.GuildModels;
using WowGuildAPI.Respository.GuildRespository.Interfaces;
using WowGuildAPI.Services.Interfaces;
using WowGuildAPI.Utils;

namespace WowGuildAPI.Respository.GuildRespository
{
    public class GuildRepository : IGuildRepository
    {
        private readonly IMongoCollection<Guild> _guilds;
        private readonly IRaiderIoGuildProfileService _raiderIoService;

        public GuildRepository(IMongoClient mongoClient, IRaiderIoGuildProfileService raiderIoService)
        {
            var database = mongoClient.GetDatabase("WowGuildDB");
            _guilds = database.GetCollection<Guild>("guilds");
            _raiderIoService = raiderIoService;
        }

        public async Task<Guild?> GetGuildAsync(string region, string realm, string name)
        {
            var guild = await GetGuildFromDbAsync(region, realm, name);
            if (guild != null && !WowUtils.IsUpdateRequired(guild.LastUpdated)) return guild;

            guild = await GetGuildFromApiAsync(region, realm, name);
            if (guild == null) return null;

            await SaveGuildToDbAsync(guild);
            return guild;
        }

        private async Task<Guild?> GetGuildFromApiAsync(string region, string realm, string name)
        {
            try
            {
                var guild = await _raiderIoService.GetGuildAsync(region, realm, name);
                if (guild == null) return null;

                guild.Id = WowUtils.GenerateWowId(region, realm, name);
                return guild;
            }
            catch
            {
                return null;
            }
        }

        private async Task<Guild?> GetGuildFromDbAsync(string region, string realm, string name)
        {
            var guildId = WowUtils.GenerateWowId(region, realm, name);
            return await _guilds.Find(g => g.Id == guildId).FirstOrDefaultAsync();
        }

        private async Task SaveGuildToDbAsync(Guild guild)
        {
            var existingGuild = await _guilds.Find(g => g.Id == guild.Id).FirstOrDefaultAsync();
            guild.LastUpdated = DateTime.UtcNow;

            if (existingGuild == null || HasGuildChanged(existingGuild, guild))
            {
                await _guilds.ReplaceOneAsync(g => g.Id == guild.Id, guild, new ReplaceOptions { IsUpsert = true });
            }
        }
        private bool HasGuildChanged(Guild existingGuild, Guild newGuild) =>
            existingGuild.LastCrawledAt != newGuild.LastCrawledAt;
    }
}
