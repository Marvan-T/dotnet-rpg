using dotnet_rpg.Dtos.Fight;
using dotnet_rpg.Services.AttackService;
using dotnet_rpg.Services.FightLogger;

namespace dotnet_rpg.Tests.Services.Test_Helpers;

public static class FightServiceTestHelper
{
    public static void AssertLogVictory(Mock<IFightLogger> fightLoggerMock, List<Character> characters, Times times)
    {
        fightLoggerMock.Verify(l => l.LogVictory(characters[0], characters[1], It.IsAny<FightResultDto>()), times);
    }

    public static void AssertWeaponAttack(Mock<IAttackService> attackServiceMock, Character attacker,
        Character opponent, Times times)
    {
        attackServiceMock.Verify(a => a.DoWeaponAttack(attacker, opponent), times);
    }

    public static void AssertSkillAttack(Mock<IAttackService> attackServiceMock, Character attacker, Character opponent,
        int skillId, Times times)
    {
        attackServiceMock.Verify(a => a.DoSkillAttack(attacker, opponent, skillId), times);
    }

    public static void AssertSkipAttack(Mock<IAttackService> attackServiceMock, Character attacker, Character opponent,
        Times times)
    {
        attackServiceMock.Verify(a => a.SkipAttack(attacker, opponent), times);
    }


    public static void AssertLog(Mock<IFightLogger> fightLoggerMock, List<Character> characters, AttackType attackType,
        Times times, int damage = 0)
    {
        // Specific assertions for `FightResultDto` isn't necessary as this is just a string of logs that is passed in
        if (attackType is AttackType.Skill or AttackType.Weapon)
            fightLoggerMock.Verify(l => l.LogAttack(
                characters[0],
                characters[1],
                damage,
                attackType,
                It.IsAny<FightResultDto>()), times);
        else
            fightLoggerMock.Verify(l => l.LogSkipTurn(characters[0], It.IsAny<FightResultDto>()), times);
    }
}