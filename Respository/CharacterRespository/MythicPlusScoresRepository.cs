using MongoDB.Driver;
using WowGuildAPI.Models.CharacterModels;
using WowGuildAPI.Respository.CharacterRespository.Interfaces;
using WowGuildAPI.Services.Interfaces;
using WowGuildAPI.Utils;

namespace WowGuildAPI.Respository.CharacterRespository
{
    public class MythicPlusScoresRepository : IMythicPlusScoresRepository
    {
        private readonly IMongoCollection<Character> _characters;
        private readonly IRaiderIoCharacterProfileService _raiderIoService;
        private readonly ICharacterRepository _characterRep;

        public MythicPlusScoresRepository(IMongoClient mongoClient, IRaiderIoCharacterProfileService raiderIoService, ICharacterRepository characterRep)
        {
            var database = mongoClient.GetDatabase("WowGuildDB");
            _characters = database.GetCollection<Character>("characters");
            _raiderIoService = raiderIoService;
            _characterRep = characterRep;
        }

        public async Task<List<MythicPlusScores>?> GetMythicPlusScoresAsync(string region, string realm, string name)
        {
            var character = await _characterRep.GetCharacterAsync(region, realm, name);
            if (character == null) return null;

            var mythicPlusScores = await GetMythicPlusScoresFromDbAsync(region, realm, name);

            if (mythicPlusScores != null && mythicPlusScores.Count > 0 && !WowUtils.IsUpdateRequired(character.LastUpdated))
            {
                return mythicPlusScores;
            }

            mythicPlusScores = await GetMythicPlusScoresFromApiAsync(region, realm, name);
            if (mythicPlusScores == null) return null;

            await SaveMythicPlusScoresToDbAsync(character, mythicPlusScores);
            return mythicPlusScores;

        }
        private async Task<List<MythicPlusScores>?> GetMythicPlusScoresFromApiAsync(string region, string realm, string name)
        {
            try
            {
                return await _raiderIoService.GetMythicPlusScoresAsync(region, realm, name) is { Count: > 0 } mythicPlusScores ? mythicPlusScores : null;
            }
            catch
            {
                return null;
            }

        }

        private async Task<List<MythicPlusScores>?> GetMythicPlusScoresFromDbAsync(string region, string realm, string name)
        {
            var characterId = WowUtils.GenerateWowId(region, realm, name);
            return (await _characters.Find(c => c.Id == characterId).FirstOrDefaultAsync())?.MythicPlusScores;

        }

        private async Task SaveMythicPlusScoresToDbAsync(Character character, List<MythicPlusScores> mythicPlusScores)
        {
            if (character.MythicPlusScores?.SequenceEqual(mythicPlusScores) == true) return;

            character.MythicPlusScores = mythicPlusScores;
            character.LastUpdated = DateTime.Now;
            await _characters.ReplaceOneAsync(c => c.Id == character.Id, character, new ReplaceOptions { IsUpsert = true });
        }

    }
}
