using dotnet_rpg.Dtos.Fight;

namespace dotnet_rpg.Services.FightService;

public interface IFightService
{
    Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto weaponAttackDto);
    Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackDto skillAttackDto);
    Task<ServiceResponse<FightResultDto>> Fight(FightRequestDto fightRequestDto);
}