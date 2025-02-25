using AutoMapper;
using WowGuildAPI.Models.CharacterModels;
using WowGuildAPI.Models.CharacterModels.Dtos;

namespace WowGuildAPI.WowMappers
{
    public class CharacterMapper:Profile
    {
        public CharacterMapper()
        {
            CreateMap<Character, CharacterDto>().ReverseMap();
            CreateMap<MythicPlusScores, MythicPlusScoresDto>().ReverseMap();
            CreateMap<Segments, SegmentsDto>().ReverseMap();
            CreateMap<MythicPlusBestRuns, MythicPlusBestRunsDto>().ReverseMap();
            CreateMap<Affix, AffixDto>().ReverseMap();
        }
    }
}
