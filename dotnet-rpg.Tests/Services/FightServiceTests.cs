using dotnet_rpg.Dtos.Fight;
using dotnet_rpg.Repository;
using dotnet_rpg.Services.AttackPerformService;
using dotnet_rpg.Services.AttackService;
using dotnet_rpg.Services.CharacterLookupService;
using dotnet_rpg.Services.FightLogger;
using dotnet_rpg.Services.FightService;
using dotnet_rpg.Tests.Services.Test_Helpers;
using dotnet_rpg.Utility.RandomGeneration;

public class FightServiceTests
{
    private readonly Mock<IAttackPerformService> _attackPerformServiceMock = new();
    private readonly Mock<IAttackService> _attackServiceMock = new();
    private readonly Mock<ICharacterLookupService> _characterLookupServiceMock = new();
    private readonly Mock<IRepository<Character>> _characterRepositoryMock = new();
    private readonly Mock<IFightLogger> _fightLoggerMock = new();
    private readonly FightService _fightService;
    private readonly Mock<IRandomGenerator> _randomMock = new();

    public FightServiceTests()
    {
        _fightService = new FightService(
            _attackPerformServiceMock.Object,
            _characterLookupServiceMock.Object,
            _randomMock.Object,
            _characterRepositoryMock.Object,
            _fightLoggerMock.Object,
            _attackServiceMock.Object);
    }

    [Fact]
    public async Task WeaponAttack_PerformsWeaponAttack()
    {
        //Arrange
        var weaponAttackDto = new WeaponAttackDto();
        var expectedAttackResult = new ServiceResponse<AttackResultDto>();

        _attackPerformServiceMock.Setup(a => a.PerformAttack(
                weaponAttackDto, It.IsAny<Func<Character, Character, int>>()))
            .ReturnsAsync(expectedAttackResult);

        //Act
        var result = await _fightService.WeaponAttack(weaponAttackDto);

        //Assert
        result.Should().BeEquivalentTo(expectedAttackResult);
    }

    [Fact]
    public async Task SkillAttack_PerformsSkillAttack()
    {
        //Arrange
        var skillAttackDto = new SkillAttackDto { SkillId = 123 };
        var expectedAttackResult = new ServiceResponse<AttackResultDto>();

        _attackPerformServiceMock.Setup(a => a.PerformAttack(
                skillAttackDto, It.IsAny<Func<Character, Character, int>>()))
            .ReturnsAsync(expectedAttackResult);

        //Act
        var result = await _fightService.SkillAttack(skillAttackDto);

        //Assert
        result.Should().BeEquivalentTo(expectedAttackResult);
    }

    [Fact]
    public async Task Fight_ThrowsException_ReturnsErrorResponse()
    {
        var fightRequestDto = new FightRequestDto { CharacterIds = new List<int> { 1, 2 } };
        _characterLookupServiceMock.Setup(c => c.FindCharactersByIds(It.IsAny<List<int>>())) // Change made here
            .ThrowsAsync(new Exception("TestException"));

        var result = await _fightService.Fight(fightRequestDto);

        result.Success.Should().BeFalse();
        result.Message.Should().Be("TestException");
    }

    [Fact]
    public async Task Fight_AttackerExecutesWeaponAttackAndLogsVictory()
    {
        //Arrange
        var fightRequestDto = new FightRequestDto
        {
            CharacterIds = new List<int> { 1, 2 }
        };

        var (characters, expectedAttackResult) = SetupForAttack(AttackType.Weapon);

        //Act
        await _fightService.Fight(fightRequestDto);

        //Assert
        FightServiceTestHelper.AssertLog(_fightLoggerMock, characters, AttackType.Weapon, Times.Once(),
            expectedAttackResult.DamageDealt);
        FightServiceTestHelper.AssertWeaponAttack(_attackServiceMock, characters[0], characters[1], Times.Once());
        FightServiceTestHelper.AssertLogVictory(_fightLoggerMock, characters, Times.Once());
    }

    [Fact]
    public async Task Fight_AttackerExecutesSkillAttackAndLogsVictory()
    {
        //Arrange
        var fightRequestDto = new FightRequestDto
        {
            CharacterIds = new List<int> { 1, 2 }
        };

        var (characters, expectedAttackResult) = SetupForAttack(AttackType.Skill);

        //Act
        await _fightService.Fight(fightRequestDto);

        //Assert
        FightServiceTestHelper.AssertSkillAttack(_attackServiceMock, characters[0], characters[1],
            characters[0].Skills[0].Id, Times.Once());
        FightServiceTestHelper.AssertLog(_fightLoggerMock, characters, AttackType.Skill, Times.Once(),
            expectedAttackResult.DamageDealt);
        FightServiceTestHelper.AssertLogVictory(_fightLoggerMock, characters, Times.Once());
    }


    [Fact]
    public async Task Fight_ExecutesSkipAttackAndLogsSkipTurn_WhenRandomlySelected()
    {
        //Arrange
        var fightRequestDto = new FightRequestDto
        {
            CharacterIds = new List<int> { 1, 2 }
        };

        var attackerWithNoSkillOrWeapon = new Character
        {
            Id = 1,
            HitPoints = 100,
            Weapon = null,
            Skills = new List<Skill>()
        };
        var (characters, _) = SetupForAttack(AttackType.Skip, attackerWithNoSkillOrWeapon);

        //Act
        await _fightService.Fight(fightRequestDto);

        //Assert
        FightServiceTestHelper.AssertSkipAttack(_attackServiceMock, characters[0], characters[1], Times.Once());
        FightServiceTestHelper.AssertLog(_fightLoggerMock, characters, AttackType.Skip, Times.Once());
        FightServiceTestHelper.AssertLogVictory(_fightLoggerMock, characters, Times.Never());
    }

    private (List<Character>, AttackResultDto?) SetupForAttack(AttackType type, Character? attacker = null,
        int opponentHP = 5, int damageDealt = 5)
    {
        var defaultAttacker = new Character
        {
            Id = 1,
            HitPoints = 100,
            Weapon = new Weapon(),
            Skills = new List<Skill> { new() { Id = 1 } }
        };

        var characters = new List<Character>
        {
            attacker ?? defaultAttacker,
            new() { Id = 2, HitPoints = opponentHP, Weapon = null, Skills = new List<Skill>() }
        };

        _characterLookupServiceMock.Setup(c => c.FindCharactersByIds(new List<int> { characters[0].Id, 2 }))
            .ReturnsAsync(characters);

        switch (type)
        {
            case AttackType.Weapon:
                _randomMock.Setup(r => r.Next(characters.Count - 1)).Returns(0);
                _randomMock.Setup(r => r.Next(2)).Returns(0); //Selecting Weapon attack
                _attackServiceMock.Setup(a => a.DoWeaponAttack(characters[0], characters[1]))
                    .Callback((Character _, Character defender) => { defender.HitPoints = 0; });
                break;
            case AttackType.Skill:
                _randomMock.Setup(r => r.Next(characters.Count - 1)).Returns(0);
                _randomMock.Setup(r => r.Next(2)).Returns(1); //Selecting Skill attack
                _randomMock.Setup(r => r.Next(characters[1].Skills.Count)).Returns(0);
                _attackServiceMock.Setup(a => a.DoSkillAttack(characters[0], characters[1], characters[0].Skills[0].Id))
                    .Callback((Character _, Character defender, int _) => { defender.HitPoints = 0; });
                break;
            case AttackType.Skip:
                _randomMock.SetupSequence(r => r.Next(characters.Count - 1)).Returns(0)
                    .Throws(new Exception("LoopTerminationForTestException"));
                // since character does not have a weapon or skill, the Skip attack type should be chosen
                break;
        }

        var attackResultDto = new AttackResultDto { DamageDealt = damageDealt };
        _attackPerformServiceMock.Setup(a => a.ExecuteAttack(
                It.Is<Character>(c => c == characters[0]),
                It.Is<Character>(c => c == characters[1]),
                It.IsAny<Func<Character, Character, int>>()))
            .Callback<Character, Character, Func<Character, Character, int>>((attacker, defender, attackFunc) =>
                attackFunc.Invoke(attacker, defender))
            .ReturnsAsync(attackResultDto);

        return (characters, type == AttackType.Skip ? null : attackResultDto);
    }


    [Fact]
    public async Task Fight_LogsWeaponAttack_WhenCharacterHasWeaponButNoSkills()
    {
        //Arrange
        var fightRequestDto = new FightRequestDto
        {
            CharacterIds = new List<int> { 1, 2 }
        };

        var attackerWithWeapon = new Character
        {
            Id = 1,
            HitPoints = 100,
            Weapon = new Weapon(),
            Skills = new List<Skill>()
        };

        var (characters, expectedAttackResult) = SetupForAttack(AttackType.Weapon, attackerWithWeapon);

        //Act
        await _fightService.Fight(fightRequestDto);

        //Assert
        FightServiceTestHelper.AssertLog(_fightLoggerMock, characters, AttackType.Weapon, Times.Once(),
            expectedAttackResult.DamageDealt);
    }

    [Fact]
    public async Task Fight_LogsSkillAttack_WhenCharacterHasSkillsButNoWeapon()
    {
        // Arrange
        var fightRequestDto = new FightRequestDto
        {
            CharacterIds = new List<int> { 1, 2 }
        };

        var attackerWithSkill = new Character
        {
            Id = 1,
            HitPoints = 100,
            Weapon = null,
            Skills = new List<Skill> { new() { Id = 1 } }
        };

        var (characters, expectedAttackResult) = SetupForAttack(AttackType.Skill, attackerWithSkill);

        //Act
        await _fightService.Fight(fightRequestDto);

        //Assert
        FightServiceTestHelper.AssertLog(_fightLoggerMock, characters, AttackType.Skill, Times.Once(),
            expectedAttackResult.DamageDealt);
    }
}