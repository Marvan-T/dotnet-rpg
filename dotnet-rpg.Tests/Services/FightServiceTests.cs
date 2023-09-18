using dotnet_rpg.Dtos.Fight;
using dotnet_rpg.Exceptions;
using dotnet_rpg.Services.AttackPerformService;
using dotnet_rpg.Services.FightService;
using dotnet_rpg.Utility.RandomGeneration;

public class FightServiceTests
{
    private readonly Mock<IAttackPerformService> _attackPerformServiceMock;
    private readonly FightService _fightService;
    private readonly Mock<IRandomGenerator> _randomGeneratorMock;

    public FightServiceTests()
    {
        _attackPerformServiceMock = new Mock<IAttackPerformService>();
        _randomGeneratorMock = new Mock<IRandomGenerator>();

        _fightService = new FightService(_attackPerformServiceMock.Object, _randomGeneratorMock.Object);
    }

    [Fact]
    public async Task WeaponAttack_NoWeaponFound_ThrowsNoWeaponFoundException()
    {
        // Arrange
        var weaponAttackDto = new WeaponAttackDto { AttackerId = 1, OpponentId = 2 };
        var attackerWithoutWeapon = new Character { Id = 1, Weapon = null };
        var opponent = new Character { Id = 2 };

        _attackPerformServiceMock
            .Setup(s => s.PerformAttack(weaponAttackDto,
                It.IsAny<Func<Character, Character, int>>()))
            .Returns<IAttackDto, Func<Character, Character, int>>((dto, func) => Task.FromResult(
                new ServiceResponse<AttackResultDto>
                {
                    Data = new AttackResultDto { DamageDealt = func.Invoke(attackerWithoutWeapon, opponent) }
                }));


        // Act 
        Func<Task> act = async () => await _fightService.WeaponAttack(weaponAttackDto);

        // Assert
        await act.Should().ThrowAsync<NoWeaponFoundException>();
    }


    [Fact]
    public async Task SkillAttack_SkillNotFound_ThrowsSkillNotFoundException()
    {
        // Arrange
        var skillAttackDto = new SkillAttackDto { AttackerId = 1, OpponentId = 2, SkillId = 999 };
        var attackerWithoutSkill = new Character { Id = 1 };
        var opponent = new Character { Id = 2 };

        _attackPerformServiceMock
            .Setup(s => s.PerformAttack(skillAttackDto, It.IsAny<Func<Character, Character, int>>()))
            .Returns<IAttackDto, Func<Character, Character, int>>((dto, func) => Task.FromResult(
                new ServiceResponse<AttackResultDto>
                {
                    Data = new AttackResultDto { DamageDealt = func.Invoke(attackerWithoutSkill, opponent) }
                }));

        // Act 
        Func<Task> act = async () => await _fightService.SkillAttack(skillAttackDto);

        // Assert 
        await act.Should().ThrowAsync<SkillNotFoundException>();
    }

    [Fact]
    public async Task WeaponAttack_ValidAttack_ComputesDamageCorrectly()
    {
        // Arrange
        var weaponAttackDto = new WeaponAttackDto { AttackerId = 1, OpponentId = 2 };
        var attacker = new Character { Id = 1, Weapon = new Weapon { Damage = 5 }, Strength = 10 };
        var opponent = new Character { Id = 2, Defense = 5, HitPoints = 100 };

        _randomGeneratorMock.Setup(r => r.Next(attacker.Strength))
            .Returns(5); // simulate a random of 5 for attacker strength
        _randomGeneratorMock.Setup(r => r.Next(opponent.Defense))
            .Returns(2); // simulate a random of 2 for opponent defense

        _attackPerformServiceMock
            .Setup(s => s.PerformAttack(weaponAttackDto, It.IsAny<Func<Character, Character, int>>()))
            .Returns<IAttackDto, Func<Character, Character, int>>((dto, func) => Task.FromResult(
                new ServiceResponse<AttackResultDto>
                {
                    Data = new AttackResultDto { DamageDealt = func.Invoke(attacker, opponent) }
                }));

        // Act
        var result = await _fightService.WeaponAttack(weaponAttackDto);

        // Assert
        result.Data.DamageDealt.Should().Be(8); // 5 base + 5 random - 2 defense = 8
        opponent.HitPoints.Should().Be(92); // 100 - 8 = 92
    }

    [Fact]
    public async Task SkillAttack_ValidAttack_ComputesDamageCorrectly()
    {
        // Arrange
        var skillAttackDto = new SkillAttackDto { AttackerId = 1, OpponentId = 2, SkillId = 1 };
        var skill = new Skill { Id = 1, Damage = 5 };
        var attacker = new Character
        {
            Id = 1, Skills = new List<Skill> { skill }, Intelligence = 10
        }; // Assuming that you use Intelligence or some other property for skill modifier.
        var opponent = new Character { Id = 2, Defense = 5, HitPoints = 100 };

        _randomGeneratorMock.Setup(r => r.Next(attacker.Intelligence))
            .Returns(5); // simulate a random of 5 for attacker intelligence
        _randomGeneratorMock.Setup(r => r.Next(opponent.Defense))
            .Returns(2); // simulate a random of 2 for opponent defense

        _attackPerformServiceMock
            .Setup(s => s.PerformAttack(skillAttackDto, It.IsAny<Func<Character, Character, int>>()))
            .Returns<IAttackDto, Func<Character, Character, int>>((dto, func) => Task.FromResult(
                new ServiceResponse<AttackResultDto>
                {
                    Data = new AttackResultDto { DamageDealt = func.Invoke(attacker, opponent) }
                }));

        // Act
        var result = await _fightService.SkillAttack(skillAttackDto);

        // Assert
        result.Data.DamageDealt.Should().Be(8); // 5 base (from skill) + 5 random (from intelligence) - 2 defense = 8
        opponent.HitPoints.Should().Be(92); // 100 - 8 = 92
    }
}