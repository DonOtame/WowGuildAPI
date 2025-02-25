using AutoMapper;
using WowGuildAPI.Models.GuildModels;
using WowGuildAPI.Models.GuildModels.Dtos;

namespace WowGuildAPI.WowMappers
{
    public class GuildMapper : Profile
    {
        public GuildMapper()
        {
            CreateMap<Guild, GuildDto>().ReverseMap();
            CreateMap<RaidProgression,RaidProgressionDto>().ReverseMap();
            CreateMap<BossKilled, BossKilledDto>().ReverseMap();
            CreateMap<RaidRanking, RaidRankingDto>().ReverseMap();
            CreateMap<RaidLevel, RaidLevelDto>().ReverseMap();
            CreateMap<RaidEncounter,RaidEncounterDto>().ReverseMap();
            CreateMap<Member, MemberDto>().ReverseMap();
            CreateMap<Character, CharacterDto>().ReverseMap();
        }
    }
}
