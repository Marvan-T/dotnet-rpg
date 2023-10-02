using dotnet_rpg.Dtos.Fight;

namespace dotnet_rpg.Services.AttackPerformService;

public interface IAttackPerformService
{
    Task<ServiceResponse<AttackResultDto>> PerformAttack(IAttackDto attackDto,
        Func<Character, Character, int> attackStrategy);

    Task<AttackResultDto> ExecuteAttack(Character attacker, Character opponent,
        Func<Character, Character, int> attackStrategy);

}