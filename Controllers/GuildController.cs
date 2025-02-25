using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WowGuildAPI.Models.GuildModels.Dtos;
using WowGuildAPI.Respository.GuildRespository.Interfaces;
using WowGuildAPI.Services.Interfaces;
using WowGuildAPI.Utils;

namespace WowGuildAPI.Controllers
{
    [Route("api/guild")]
    [ApiController]
    public class GuildController : ControllerBase
    {
        private readonly IGuildRepository _guildRep;
        private readonly IRaidProgressionsRepository _raidProgressionRep;
        private readonly IRaidRankingsRepository _raidRankingRep;
        private readonly IMembersRepository _membersRep;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        public GuildController(
            IGuildRepository guildRep,
            IRaidProgressionsRepository raidProgressionRep,
            IRaidRankingsRepository raidRankingRep,
            IMembersRepository membersRep,
            IMapper mapper,
            ICacheService cacheService)
        {
            _guildRep = guildRep;
            _raidProgressionRep = raidProgressionRep;
            _raidRankingRep = raidRankingRep;
            _membersRep = membersRep;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        [AllowAnonymous]
        [HttpGet("profile")]
        [SwaggerOperation(
            Summary = "Retrieves guild information",
            Description = "Returns the basic details of a guild, including name, region, realm, faction, and profile URL.")]
        public async Task<IActionResult> GetGuild(
            [FromQuery][Required][SwaggerParameter(Description = "The region where the guild is located. Allowed values: us, eu, tw, kr, cn.")] string region,
            [FromQuery][Required][SwaggerParameter(Description = "The realm in which the guild exists. E.g., 'Draenor', 'Stormrage'.")] string realm,
            [FromQuery][Required][SwaggerParameter(Description = "The name of the guild to retrieve.")] string name)
        {
            var validationMessage = InputValidation.ValidateRequest(region, name, realm);
            if (validationMessage != null)
                return BadRequest(validationMessage);

            var cacheKey = $"guild:{region}:{realm}:{name}";
            var guildDto = await _cacheService.GetOrCreateAsync(cacheKey,
                async () =>
                {
                    var guild = await _guildRep.GetGuildAsync(region, realm, name);
                    if (guild == null)
                        return null;

                    return _mapper.Map<GuildDto>(guild);
                });

            if (guildDto == null)
                return NotFound("Guild not found in database or Raider.IO.");

            return Ok(guildDto);
        }

        [AllowAnonymous]
        [HttpGet("raid-progressions")]
        [SwaggerOperation(
            Summary = "Retrieves raid progressions for a guild",
            Description = "Returns the raid progression details for a specified guild, showing the furthest boss defeated at each difficulty level.")]
        public async Task<IActionResult> GetRaidProgressions(
            [FromQuery][Required][SwaggerParameter(Description = "The region where the guild is located. Allowed values: us, eu, tw, kr, cn.")] string region,
            [FromQuery][Required][SwaggerParameter(Description = "The realm in which the guild exists. E.g., 'Draenor', 'Stormrage'.")] string realm,
            [FromQuery][Required][SwaggerParameter(Description = "The name of the guild to retrieve.")] string name)
        {
            var validationMessage = InputValidation.ValidateRequest(region, name, realm);
            if (validationMessage != null)
                return BadRequest(validationMessage);

            var cacheKey = $"raid-progressions:{region}:{realm}:{name}";
            var raidProgressionsDto = await _cacheService.GetOrCreateAsync(cacheKey,
                async () =>
                {
                    var raidProgressions = await _raidProgressionRep.GetRaidProgressionAsync(region, realm, name);
                    return _mapper.Map<Dictionary<string, RaidProgressionDto>>(raidProgressions);
                });

            if (raidProgressionsDto == null)
                return NotFound("Raid progressions not found in database or Raider.IO.");

            return Ok(raidProgressionsDto);
        }

        [AllowAnonymous]
        [HttpGet("raid-rankings")]
        [SwaggerOperation(
            Summary = "Retrieves raid rankings for a guild",
            Description = "Returns the raid rankings for a specified guild, including rankings at normal, heroic, and mythic levels.")]
        public async Task<IActionResult> GetRaidRankings(
            [FromQuery][Required][SwaggerParameter(Description = "The region where the guild is located. Allowed values: us, eu, tw, kr, cn.")] string region,
            [FromQuery][Required][SwaggerParameter(Description = "The realm in which the guild exists. E.g., 'Draenor', 'Stormrage'.")] string realm,
            [FromQuery][Required][SwaggerParameter(Description = "The name of the guild to retrieve.")] string name)
        {
            var validationMessage = InputValidation.ValidateRequest(region, name, realm);
            if (validationMessage != null)
                return BadRequest(validationMessage);

            var cacheKey = $"raid-rankings:{region}:{realm}:{name}";
            var raidRankingsDto = await _cacheService.GetOrCreateAsync(cacheKey,
                async () =>
                {
                    var raidRankings = await _raidRankingRep.GetRaidRankingsAsync(region, realm, name);
                    return _mapper.Map<Dictionary<string, RaidRankingDto>>(raidRankings);
                });

            if (raidRankingsDto == null)
                return NotFound("Raid rankings not found in database or Raider.IO.");

            return Ok(raidRankingsDto);
        }

        [AllowAnonymous]
        [HttpGet("members")]
        [SwaggerOperation(
            Summary = "Retrieves the guild members",
            Description = "Returns the members of a guild where the rank is Guild Master (0), and the next two ranks (1, 2).")]
        public async Task<IActionResult> GetMembers(
            [FromQuery][Required][SwaggerParameter(Description = "The region where the guild is located. Allowed values: us, eu, tw, kr, cn.")] string region,
            [FromQuery][Required][SwaggerParameter(Description = "The realm in which the guild exists. E.g., 'Draenor', 'Stormrage'.")] string realm,
            [FromQuery][Required][SwaggerParameter(Description = "The name of the guild to retrieve.")] string name)
        {
            var validationMessage = InputValidation.ValidateRequest(region, name, realm);
            if (validationMessage != null)
                return BadRequest(validationMessage);

            var cacheKey = $"members:{region}:{realm}:{name}";
            var membersDto = await _cacheService.GetOrCreateAsync(cacheKey,
                async () =>
                {
                    var members = await _membersRep.GetMembersAsync(region, realm, name);
                    return _mapper.Map<List<MemberDto>>(members);
                });

            if (membersDto == null)
                return NotFound("Members not found in database or Raider.IO.");

            return Ok(membersDto);
        }
    }
}
