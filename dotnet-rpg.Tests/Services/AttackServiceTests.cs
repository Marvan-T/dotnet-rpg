using dotnet_rpg.Exceptions;
using dotnet_rpg.Services.AttackService;
using dotnet_rpg.Utility.RandomGeneration;

namespace dotnet_rpg.Tests.Services;

public class AttackServiceTests
{
    private readonly Mock<IRandomGenerator> _randomGeneratorMock;
    private readonly IAttackService _attackService;

    public AttackServiceTests()
    {
        _randomGeneratorMock = new Mock<IRandomGenerator>();
        _attackService = new AttackService(_randomGeneratorMock.Object);
    }

    [Fact]
    public void DoWeaponAttack_ShouldThrowNoWeaponFoundException_WhenAttackerHasNoWeapon()
    {
        var attacker = new Character { Weapon = null, Id = 1 };
        var opponent = new Character();

        _attackService.Invoking(service => service.DoWeaponAttack(attacker, opponent))
            .Should().Throw<NoWeaponFoundException>()
            .WithMessage($"Attacker with ID {attacker.Id} does not have a weapon.");
    }

    [Fact]
    public void DoSkillAttack_ShouldThrowSkillNotFoundException_WhenAttackerDoesNotHaveSkill()
    {
        var attacker = new Character { Skills = new Skill[] { } };
        var opponent = new Character();
        int nonExistentSkillId = 999;

        _attackService.Invoking(service => service.DoSkillAttack(attacker, opponent, nonExistentSkillId))
            .Should().Throw<SkillNotFoundException>()
            .WithMessage($"Skill with id: {nonExistentSkillId} not found.");
    }

    [Fact]
    public void SkipAttack_ShouldAlwaysReturnZero()
    {
        var attacker = new Character();
        var opponent = new Character();

        int result = _attackService.SkipAttack(attacker, opponent);

        result.Should().Be(0);
    }
    
    [Fact]
    public void DoWeaponAttack_ShouldCalculateDamageCorrectly()
    {
        int baseDamage = 10;
        int attackerStrength = 5;
        int opponentDefense = 3;
        var attacker = new Character { Weapon = new Weapon { Damage = baseDamage }, Strength = attackerStrength };
        var opponent = new Character { Defense = opponentDefense, HitPoints = 100 };
    
        _randomGeneratorMock.Setup(r => r.Next(attackerStrength)).Returns(4); // random value for the attacker
        _randomGeneratorMock.Setup(r => r.Next(opponentDefense)).Returns(2);   // random value for the opponent

        int expectedDamage = baseDamage + 4 - 2;  // 10 + 4 - 2 = 12
        var result = _attackService.DoWeaponAttack(attacker, opponent);

        result.Should().Be(expectedDamage);
        opponent.HitPoints.Should().Be(100 - expectedDamage);  // 100 - 12 = 88
    }

    [Fact]
    public void DoSkillAttack_ShouldCalculateDamageCorrectly()
    {
        int baseDamage = 10;
        int attackerIntelligence = 5;
        int opponentDefense = 3;
        int skillId = 1;
        var attacker = new Character { Skills = new[] { new Skill { Id = skillId, Damage = baseDamage } }, Intelligence = attackerIntelligence };
        var opponent = new Character { Defense = opponentDefense, HitPoints = 100 };
    
        _randomGeneratorMock.Setup(r => r.Next(attackerIntelligence)).Returns(4); // random value for the attacker
        _randomGeneratorMock.Setup(r => r.Next(opponentDefense)).Returns(2);   // random value for the opponent

        int expectedDamage = baseDamage + 4 - 2;  // 10 + 4 - 2 = 12
        var result = _attackService.DoSkillAttack(attacker, opponent, skillId);

        result.Should().Be(expectedDamage);
        opponent.HitPoints.Should().Be(100 - expectedDamage);  // 100 - 12 = 88
    }

}