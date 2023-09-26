namespace dotnet_rpg.Services.AttackService;

public interface IAttackService
{
    int DoWeaponAttack(Character attacker, Character opponent);
    int DoSkillAttack(Character attacker, Character opponent, int skillId);
    int SkipAttack(Character attacker, Character opponent);
}