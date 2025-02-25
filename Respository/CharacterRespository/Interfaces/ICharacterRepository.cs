using WowGuildAPI.Models.CharacterModels;

namespace WowGuildAPI.Respository.CharacterRespository.Interfaces
{
    public interface ICharacterRepository
    {
        Task<Character?> GetCharacterAsync(string region, string realm, string name);
    }
}
