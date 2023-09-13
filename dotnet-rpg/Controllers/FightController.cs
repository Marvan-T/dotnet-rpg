using dotnet_rpg.Dtos.Fight;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FightController : ControllerBase
{
    private readonly IFightService _fightService;

    public FightController(IFightService fightService)
    {
        _fightService = fightService;
    }

    [HttpPost]
    public async Task<ActionResult<ServiceResponse<AttackResultDto>>> DoWeaponAttack(WeaponAttackDto weaponAttackDto)
    {
        var response = await _fightService.WeaponAttack(weaponAttackDto);
        if (!response.Success) return BadRequest(response);
        return Ok(response);
    }
}