using MongoDB.Driver;
using WowGuildAPI.Models.CharacterModels;
using WowGuildAPI.Respository.CharacterRespository.Interfaces;
using WowGuildAPI.Services.Interfaces;
using WowGuildAPI.Utils;

namespace WowGuildAPI.Respository.CharacterRespository
{
    public class CharacterRepository : ICharacterRepository
    {
        private readonly IMongoCollection<Character> _characters;
        private readonly IRaiderIoCharacterProfileService _raiderIoService;

        public CharacterRepository(IMongoClient mongoClient, IRaiderIoCharacterProfileService raiderIoService)
        {
            var database = mongoClient.GetDatabase("WowGuildDB");
            _characters = database.GetCollection<Character>("characters");
            _raiderIoService = raiderIoService;
        }

        public async Task<Character?> GetCharacterAsync(string region, string realm, string name)
        {
            var character = await GetCharacterFromDbAsync(region, realm, name);
            if (character != null && !WowUtils.IsUpdateRequired(character.LastUpdated)) return character;
            character = await GetCharacterFromApiAsync(region, realm, name);
            if (character == null) return null;
            await SaveCharacterToDbAsync(character);
            return character;
        }
        private async Task<Character?> GetCharacterFromApiAsync(string region, string realm, string name)
        {
            try
            {
                var character = await _raiderIoService.GetCharacterAsync(region, realm, name);
                if (character == null) return null;
                character.Id = WowUtils.GenerateWowId(region, realm, name);
                return character;
            }
            catch
            {
                return null;
            }
        }
        private async Task<Character?> GetCharacterFromDbAsync(string region, string realm, string name)
        {
            var characterId = WowUtils.GenerateWowId(region, realm, name);
            return await _characters.Find(c => c.Id == characterId).FirstOrDefaultAsync();
        }
        private async Task SaveCharacterToDbAsync(Character character)
        {
            var existingCharacter = await _characters.Find(c => c.Id == character.Id).FirstOrDefaultAsync();
            character.LastUpdated = DateTime.UtcNow;
            if (existingCharacter == null || HasCharacterChanged(existingCharacter, character))
            {
                if (existingCharacter != null)
                {
                    character.Id = existingCharacter.Id;
                    await _characters.ReplaceOneAsync(c => c.Id == character.Id, character);
                }
                else
                {
                    await _characters.InsertOneAsync(character);
                }
            }
        }

        private bool HasCharacterChanged(Character existingCharacter, Character newCharacter) =>
            existingCharacter.LastCrawledAt != newCharacter.LastCrawledAt;
    }
}
