using dotnet_rpg.Services.SkillService;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SkillController : ControllerBase
{
    private readonly ISkillService _skillService;

    public SkillController(ISkillService skillService)
    {
        _skillService = skillService;
    }

    [HttpPost("{characterId}")]
    public async Task<ActionResult<ServiceResponse<GetCharacterResponseDto>>> AddSkillToCharacter(int characterId,
        AddCharacterSkillDto addCharacterSkillDto)
    {
        if (characterId != addCharacterSkillDto.CharacterId) return BadRequest("Character Id's don't match");
        var response = await _skillService.AddSkillToCharacter(addCharacterSkillDto);
        if (!response.Success) return BadRequest(response);
        return Ok(response);
    }
}