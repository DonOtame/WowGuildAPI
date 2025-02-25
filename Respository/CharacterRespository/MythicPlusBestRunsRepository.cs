using MongoDB.Driver;
using WowGuildAPI.Models.CharacterModels;
using WowGuildAPI.Respository.CharacterRespository.Interfaces;
using WowGuildAPI.Services;
using WowGuildAPI.Services.Interfaces;
using WowGuildAPI.Utils;

namespace WowGuildAPI.Respository.CharacterRespository
{
    public class MythicPlusBestRunsRepository : IMythicPlusBestRunsRepository
    {
        private readonly IMongoCollection<Character> _characters;
        private readonly IRaiderIoCharacterProfileService _raiderIoService;
        private readonly ICharacterRepository _characterRep;

        public MythicPlusBestRunsRepository(IMongoClient mongoClient, IRaiderIoCharacterProfileService raiderIoService, ICharacterRepository characterRep)
        {
            var database = mongoClient.GetDatabase("WowGuildDB");
            _characters = database.GetCollection<Character>("characters");
            _raiderIoService = raiderIoService;
            _characterRep = characterRep;
        }

        public async Task<List<MythicPlusBestRuns>?> GetMythicPlusBestRunsAsync(string region, string realm, string name)
        {
            var character = await _characterRep.GetCharacterAsync(region, realm, name);
            if (character == null) return null;

            var mythicPlusBestRuns = await GetMythicPlusBestRunsFromDbAsync(region, realm, name);

            if (mythicPlusBestRuns != null && mythicPlusBestRuns.Count > 0 && !WowUtils.IsUpdateRequired(character.LastUpdated))
            {
                return mythicPlusBestRuns;
            }

            mythicPlusBestRuns = await GetMythicPlusBestRunsFromApiAsync(region, realm, name);
            if (mythicPlusBestRuns == null) return null;

            await SaveMythicPlusBestRunsToDbAsync(character, mythicPlusBestRuns);
            return mythicPlusBestRuns;
        }


        private async Task<List<MythicPlusBestRuns>?> GetMythicPlusBestRunsFromApiAsync(string region, string realm, string name)
        {
            try
            {
                return await _raiderIoService.GetMythicPlusBestRunsAsync(region, realm, name) is { Count: > 0 } mythicPlusBestRuns ? mythicPlusBestRuns : null;
            }
            catch
            {
                return null;
            }
        }

        private async Task<List<MythicPlusBestRuns>?> GetMythicPlusBestRunsFromDbAsync(string region, string realm, string name)
        {
            var characterId = WowUtils.GenerateWowId(region, realm, name);
            return (await _characters.Find(c => c.Id == characterId).FirstOrDefaultAsync())?.MythicPlusBestRuns;
        }

        private async Task SaveMythicPlusBestRunsToDbAsync(Character character, List<MythicPlusBestRuns> mythicPlusBestRuns)
        {
            if (character.MythicPlusBestRuns?.SequenceEqual(mythicPlusBestRuns) == true) return;

            character.MythicPlusBestRuns = mythicPlusBestRuns;
            character.LastUpdated = DateTime.Now;
            await _characters.ReplaceOneAsync(c => c.Id == character.Id, character, new ReplaceOptions { IsUpsert = true });
        }
    }
}