namespace dotnet_rpg.Services.AttackService;

public class AttackService : IAttackService
{
    private readonly IRandomGenerator _random;

    public AttackService(IRandomGenerator random)
    {
        _random = random;
    }

    public int DoWeaponAttack(Character attacker, Character opponent)
    {
        if (attacker.Weapon is null)
            throw new NoWeaponFoundException(attacker.Id);

        return ApplyAttack(attacker.Weapon.Damage, attacker.Strength, opponent.Defense, opponent);
    }

    public int DoSkillAttack(Character attacker, Character opponent, int skillId)
    {
        var skill = attacker.Skills.FirstOrDefault(s => s.Id == skillId);
        if (skill is null)
            throw new SkillNotFoundException(skillId);

        return ApplyAttack(skill.Damage, attacker.Intelligence, opponent.Defense, opponent);
    }

    public int SkipAttack(Character attacker, Character opponent)
    {
        return 0; // No damage is dealt in a skip action
    }

    private int ApplyAttack(int baseDamage, int attackerModifier, int opponentDefense,
        Character opponent)
    {
        var damage = baseDamage + _random.Next(attackerModifier);
        damage -= _random.Next(opponentDefense);

        if (damage > 0) opponent.HitPoints -= damage;

        return damage;
    }
}