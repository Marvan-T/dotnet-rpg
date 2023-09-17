using dotnet_rpg.Dtos.Fight;
using dotnet_rpg.Services.AttackPerformService;
using dotnet_rpg.Utility.RandomGeneration;

namespace dotnet_rpg.Services.FightService;

public class FightService : IFightService
{
    private readonly IAttackPerformService _attackPerformService;
    private readonly IRandomGenerator _random;

    public FightService(IAttackPerformService attackPerformService,
        IRandomGenerator random)
    {
        _attackPerformService = attackPerformService;
        _random = random;
    }

    public async Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto weaponAttackDto)
    {
        return await _attackPerformService.PerformAttack(weaponAttackDto,
            (attacker, opponent) => DoWeaponAttack(attacker, opponent));
    }

    public async Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackDto skillAttackDto)
    {
        return await _attackPerformService.PerformAttack(skillAttackDto,
            (attacker, opponent) => DoSkillAttack(attacker, opponent, skillAttackDto.SkillId));
    }

    private int DoWeaponAttack(Character attacker, Character opponent)
    {
        if (attacker.Weapon is null)
            throw new NoWeaponFoundException(attacker.Id);

        var damage = attacker.Weapon.Damage + _random.Next(attacker.Strength);
        damage -= _random.Next(opponent.Defense);

        if (damage > 0) opponent.HitPoints -= damage;

        return damage;
    }

    private int DoSkillAttack(Character attacker, Character opponent, int skillId)
    {
        var skill = attacker.Skills.FirstOrDefault(s => s.Id == skillId);
        if (skill is null)
            throw new SkillNotFoundException(skillId);

        var damage = skill.Damage + _random.Next(attacker.Intelligence);
        damage -= _random.Next(opponent.Defense);

        if (damage > 0) opponent.HitPoints -= damage;

        return damage;
    }
}