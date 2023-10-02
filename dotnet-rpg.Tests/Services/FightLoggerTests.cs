using dotnet_rpg.Dtos.Fight;
using dotnet_rpg.Services.FightLogger;

namespace dotnet_rpg.Tests.Services;

public class FightLoggerTests
{
    private readonly Character _characterA = new() { Name = "CharacterA" };
    private readonly Character _characterB = new() { Name = "CharacterB" };
    private readonly IFightLogger _fightLogger = new FightLogger();
    private readonly FightResultDto _fightResult = new() { Log = new List<string>() };

    [Fact]
    public void LogSkipTurn_AddsSkipMessageToLog()
    {
        _fightLogger.LogSkipTurn(_characterA, _fightResult);

        _fightResult.Log.Should().HaveCount(1);
        _fightResult.Log[0].Should().Be("CharacterA decided to skip the turn.");
    }

    [Fact]
    public void LogVictory_AddsVictoryMessagesToLog()
    {
        _fightLogger.LogVictory(_characterA, _characterB, _fightResult);

        _fightResult.Log.Should().HaveCount(2);
        _fightResult.Log[0].Should().Be("CharacterB is defeated by CharacterA");
        _fightResult.Log[1].Should().Be("CharacterA is victorious!");
    }

    [Theory]
    [InlineData(10, AttackType.Weapon, "CharacterA deals 10 to CharacterB with a Weapon attack.")]
    [InlineData(0, AttackType.Skill, "CharacterA wasn't able to deal damage to CharacterB with a Skill attack.")]
    public void LogAttack_AddsAttackMessageToLog(int damage, AttackType attackType, string expectedMessage)
    {
        _fightLogger.LogAttack(_characterA, _characterB, damage, attackType, _fightResult);

        _fightResult.Log.Should().HaveCount(1);
        _fightResult.Log[0].Should().Be(expectedMessage);
    }
}