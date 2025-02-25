using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WowGuildAPI.Models.CharacterModels.Dtos;
using WowGuildAPI.Models.Enums;
using WowGuildAPI.Respository.CharacterRespository.Interfaces;
using WowGuildAPI.Services.Interfaces;
using WowGuildAPI.Utils;

namespace WowGuildAPI.Controllers
{
    [Route("api/character")]
    [ApiController]
    public class CharacterController : ControllerBase
    {
        private readonly ICharacterRepository _charRep;
        private readonly IMythicPlusScoresRepository _mythicPlusScoresRep;
        private readonly IMythicPlusBestRunsRepository _mythicPlusBestRunsRep;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        public CharacterController(
            ICharacterRepository charRep,
            IMythicPlusScoresRepository mythicPlusScoresRep,
            IMythicPlusBestRunsRepository mythicPlusBestRunsRep,
            IMapper mapper,
            ICacheService cacheService
        )
        {
            _charRep = charRep;
            _mythicPlusScoresRep = mythicPlusScoresRep;
            _mythicPlusBestRunsRep = mythicPlusBestRunsRep;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        [AllowAnonymous]
        [HttpGet("profile")]
        [SwaggerOperation(
            Summary = "Retrieves character information",
            Description = "Returns the basic details of a character, "
        )]
        public async Task<IActionResult> GetCharacter(
            [FromQuery][Required][SwaggerParameter(Description = "The region where the character is located. Allowed values: us, eu, tw, kr, cn.")] string region,
            [FromQuery][Required][SwaggerParameter(Description = "The realm in which the character exists. E.g., 'Draenor', 'Stormrage'.")] string realm,
            [FromQuery][Required][SwaggerParameter(Description = "The name of the character to retrieve.")] string name
        )
        {
            var validationMessage = InputValidation.ValidateRequest(region, name, realm);
            if (validationMessage != null)
                return BadRequest(validationMessage);

            var cacheKey = $"character:{region}:{realm}:{name}";
            var characterDto = await _cacheService.GetOrCreateAsync(cacheKey,
                async () =>
                {
                    var character = await _charRep.GetCharacterAsync(region, realm, name);
                    if (character == null)
                        return null;

                    return _mapper.Map<CharacterDto>(character);
                });

            if (characterDto == null)
                return NotFound("Character not found in database or Raider.IO.");

            return Ok(characterDto);
        }

        [AllowAnonymous]
        [HttpGet("mythic-plus-scores")]
        [SwaggerOperation(
            Summary = "Retrieves character's Mythic+ scores",
            Description = "Returns the Mythic+ scores of a character, "
        )]
        public async Task<IActionResult> GetMythicPlusScores(
            [FromQuery][Required][SwaggerParameter(Description = "The region where the character is located. Allowed values: us, eu, tw, kr, cn.")] string region,
            [FromQuery][Required][SwaggerParameter(Description = "The realm in which the character exists. E.g., 'Draenor', 'Stormrage'.")] string realm,
            [FromQuery][Required][SwaggerParameter(Description = "The name of the character to retrieve.")] string name
        )
        {
            var validationMessage = InputValidation.ValidateRequest(region, name, realm);
            if (validationMessage != null)
                return BadRequest(validationMessage);

            var cacheKey = $"mythic-plus-scores:{region}:{realm}:{name}";
            var mythicPlusScoresDto = await _cacheService.GetOrCreateAsync(cacheKey,
                async () =>
                {
                    var mythicPlusScores = await _mythicPlusScoresRep.GetMythicPlusScoresAsync(region, realm, name);
                    return _mapper.Map<List<MythicPlusScoresDto>>(mythicPlusScores);
                });

            if (mythicPlusScoresDto == null)
                return NotFound("Mythic+ scores not found in database or Raider.IO.");

            return Ok(mythicPlusScoresDto);
        }

        [AllowAnonymous]
        [HttpGet("mythic-plus-best-runs")]
        [SwaggerOperation(
            Summary = "Retrieves character's Mythic+ best runs",
            Description = "Returns the Mythic+ best runs of a character, "
        )]
        public async Task<IActionResult> GetMythicPlusBestRuns(
            [FromQuery][Required][SwaggerParameter(Description = "The region where the character is located. Allowed values: us, eu, tw, kr, cn.")] string region,
            [FromQuery][Required][SwaggerParameter(Description = "The realm in which the character exists. E.g., 'Draenor', 'Stormrage'.")] string realm,
            [FromQuery][Required][SwaggerParameter(Description = "The name of the character to retrieve.")] string name
        )
        {
            var validationMessage = InputValidation.ValidateRequest(region, name, realm);
            if (validationMessage != null)
                return BadRequest(validationMessage);

            var cacheKey = $"mythic-plus-best-runs:{region}:{realm}:{name}";
            var mythicPlusBestRunsDto = await _cacheService.GetOrCreateAsync(cacheKey,
                async () =>
                {
                    var mythicPlusBestRuns = await _mythicPlusBestRunsRep.GetMythicPlusBestRunsAsync(region, realm, name);
                    return _mapper.Map<List<MythicPlusBestRunsDto>>(mythicPlusBestRuns);
                });

            if (mythicPlusBestRunsDto == null)
                return NotFound("Mythic+ best runs not found in database or Raider.IO.");

            return Ok(mythicPlusBestRunsDto);
        }
    }
}
