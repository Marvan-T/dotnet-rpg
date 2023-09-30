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
    public async Task Fight_ExecutesWeaponAttackAndLogsVictory_WhenRandomlySelected()
    {
        //Arrange
        var fightRequestDto = new FightRequestDto
        {
            CharacterIds = new List<int> { 1, 2 }
        };

        var (characters, expectedAttackResult) = SetupForWeaponAttack();

        _randomMock.SetupSequence(r => r.Next(It.IsAny<int>()))
            .Returns(0) // For selecting AttackType.Weapon
            .Returns(0); // For selecting the opponent

        //Act
        await _fightService.Fight(fightRequestDto);

        //Assert
        FightServiceTestHelper.AssertLog(_fightLoggerMock, characters, AttackType.Weapon, Times.Once(),
            expectedAttackResult.DamageDealt);
        FightServiceTestHelper.AssertWeaponAttack(_attackServiceMock, characters[0], characters[1], Times.Once());
        FightServiceTestHelper.AssertLogVictory(_fightLoggerMock, characters, Times.Once());
    }


    private (List<Character>, AttackResultDto) SetupForWeaponAttack(int opponentHP = 5, int damageDealt = 5)
    {
        var characters = new List<Character>
        {
            new() { Id = 1, HitPoints = 100, Weapon = new Weapon(), Skills = new List<Skill>() },
            new() { Id = 2, HitPoints = opponentHP, Weapon = null, Skills = new List<Skill>() }
        };

        _characterLookupServiceMock.Setup(c => c.FindCharactersByIds(new List<int> { 1, 2 })).ReturnsAsync(characters);

        _attackServiceMock.Setup(a => a.DoWeaponAttack(characters[0], characters[1]))
            .Callback((Character _, Character defender) => { defender.HitPoints = 0; });

        var attackResultDto = new AttackResultDto { DamageDealt = damageDealt };

        _attackPerformServiceMock.Setup(a => a.ExecuteAttack(
                It.Is<Character>(c => c == characters[0]),
                It.Is<Character>(c => c == characters[1]),
                It.IsAny<Func<Character, Character, int>>()))
            .Callback<Character, Character, Func<Character, Character, int>>((attacker, defender, attackFunc) =>
                attackFunc(attacker,
                    defender)) // If the `DoWeaponAttack` function is not invoked HitPoints won't be reset to 0, so no explicit check is required for the method reference we pass
            .ReturnsAsync(new AttackResultDto { DamageDealt = damageDealt });

        return (characters, attackResultDto);
    }

    [Fact]
    public async Task Fight_ExecutesSkillAttackAndLogsVictory_WhenRandomlySelected()
    {
        //Arrange
        var fightRequestDto = new FightRequestDto
        {
            CharacterIds = new List<int> { 1, 2 }
        };

        var (characters, expectedAttackResult) = SetupForSkillAttack();

        _randomMock.SetupSequence(r => r.Next(characters.Count - 1)).Returns(0); // For selecting the opponent
        _randomMock.SetupSequence(r => r.Next(0)).Returns(1); // For Skill Attack Type
        _randomMock.SetupSequence(r => r.Next(characters[1].Skills.Count)).Returns(0); //For selecting the skill

        //Act
        await _fightService.Fight(fightRequestDto);

        //Assert
        FightServiceTestHelper.AssertSkillAttack(_attackServiceMock, characters[0], characters[1],
            characters[0].Skills[0].Id, Times.Once());
        FightServiceTestHelper.AssertLog(_fightLoggerMock, characters, AttackType.Skill, Times.Once(),
            expectedAttackResult.DamageDealt);
        FightServiceTestHelper.AssertLogVictory(_fightLoggerMock, characters, Times.Once());
    }

    private (List<Character>, AttackResultDto) SetupForSkillAttack(int opponentHP = 5, int damageDealt = 5)
    {
        var SkillID = 1;
        var characters = new List<Character>
        {
            new() { Id = 1, HitPoints = 100, Weapon = null, Skills = new List<Skill> { new() { Id = SkillID } } },
            new() { Id = 2, HitPoints = opponentHP, Weapon = null, Skills = new List<Skill>() }
        };

        _characterLookupServiceMock.Setup(c => c.FindCharactersByIds(new List<int> { 1, 2 })).ReturnsAsync(characters);
        _attackServiceMock.Setup(a => a.DoSkillAttack(characters[0], characters[1], SkillID))
            .Callback((Character _, Character defender, int _) => { defender.HitPoints = 0; });

        var attackResultDto = new AttackResultDto { DamageDealt = damageDealt };

        _attackPerformServiceMock.Setup(a => a.ExecuteAttack(
                It.Is<Character>(c => c == characters[0]),
                It.Is<Character>(c => c == characters[1]),
                It.IsAny<Func<Character, Character, int>>()))
            .Callback<Character, Character, Func<Character, Character, int>>((attacker, defender, attackFunc) =>
                attackFunc(attacker,
                    defender)) // If the `DoSkillAttack` function is not invoked HitPoints won't be reset to 0, so no explicit check is required for the method reference we pass
            .ReturnsAsync(attackResultDto);

        return (characters, attackResultDto);
    }

    [Fact]
    public async Task Fight_ExecutesSkipAttackAndLogsSkipTurn_WhenRandomlySelected()
    {
        //Arrange
        var fightRequestDto = new FightRequestDto
        {
            CharacterIds = new List<int> { 1, 2 }
        };

        var characters = SetupForSkipAttack();

        _randomMock.SetupSequence(r => r.Next(characters.Count - 1))
            .Returns(0) // First call returns the opponent
            .Throws(new Exception(
                "LoopTerminationForTestException")); // Second call throws an exception to terminate the loop
        _randomMock.SetupSequence(r => r.Next(0)).Returns(2); // For Skip Attack Type

        //Act
        await _fightService.Fight(fightRequestDto);

        //Assert
        FightServiceTestHelper.AssertSkipAttack(_attackServiceMock, characters[0], characters[1], Times.Once());
        FightServiceTestHelper.AssertLog(_fightLoggerMock, characters, AttackType.Skip, Times.Once());
        FightServiceTestHelper.AssertLogVictory(_fightLoggerMock, characters, Times.Never());
    }

    private List<Character> SetupForSkipAttack()
    {
        var characters = new List<Character>
        {
            new() { Id = 1, HitPoints = 100, Weapon = null, Skills = new List<Skill>() },
            new()
            {
                Id = 2, HitPoints = 5, Weapon = null, Skills = new List<Skill>()
            }
        };

        _characterLookupServiceMock.Setup(c => c.FindCharactersByIds(new List<int> { 1, 2 })).ReturnsAsync(characters);

        _attackPerformServiceMock.Setup(a => a.ExecuteAttack(
                It.Is<Character>(c => c == characters[0]),
                It.Is<Character>(c => c == characters[1]),
                It.IsAny<Func<Character, Character, int>>()))
            .Callback<Character, Character, Func<Character, Character, int>>((attacker, defender, attackFunc) =>
                attackFunc(attacker, defender));

        return characters;
    }

    [Fact]
    public async Task Fight_LogsWeaponAttack_WhenCharacterHasWeaponButNoSkills()
    {
        //Arrange
        var fightRequestDto = new FightRequestDto
        {
            CharacterIds = new List<int> { 1, 2 }
        };

        var characters = new List<Character>
        {
            new() { Id = 1, HitPoints = 100, Weapon = new Weapon(), Skills = new List<Skill>() },
            new() { Id = 2, HitPoints = 100, Weapon = null, Skills = new List<Skill>() },
            new() { Id = 3, HitPoints = 100, Weapon = null, Skills = new List<Skill>() }
        };

        _randomMock.SetupSequence(r => r.Next(characters.Count - 1))
            .Returns(0)
            .Throws(new Exception(
                "LoopTerminationForTestException")); // Second call throws an exception to terminate the loop; 

        _randomMock.Setup(r => r.Next(1)).Returns(0); // Select weapon attack
        _characterLookupServiceMock.Setup(c => c.FindCharactersByIds(new List<int> { 1, 2 })).ReturnsAsync(characters);

        var DamageDealt = 5;
        _attackPerformServiceMock.Setup(a =>
                a.ExecuteAttack(It.IsAny<Character>(), It.IsAny<Character>(),
                    It.IsAny<Func<Character, Character, int>>()))
            .ReturnsAsync(new AttackResultDto { DamageDealt = DamageDealt });

        //Act
        await _fightService.Fight(fightRequestDto);

        //Assert
        FightServiceTestHelper.AssertLog(_fightLoggerMock, characters, AttackType.Weapon, Times.Once(), DamageDealt);
    }

    [Fact]
    public async Task Fight_LogsSkillAttack_WhenCharacterHasSkillsButNoWeapon()
    {
        // Arrange
        var fightRequestDto = new FightRequestDto
        {
            CharacterIds = new List<int> { 1, 2 }
        };

        var characters = new List<Character>
        {
            new()
            {
                Id = 1, HitPoints = 100, Weapon = null, Skills = new List<Skill> { new() { Id = 101 } }
            }, // Character with skills but no weapon
            new() { Id = 2, HitPoints = 100, Weapon = null, Skills = new List<Skill>() },
            new() { Id = 2, HitPoints = 100, Weapon = null, Skills = new List<Skill>() }
        };

        _randomMock.SetupSequence(r => r.Next(characters.Count - 1))
            .Returns(0)
            .Throws(new Exception(
                "LoopTerminationForTestException"));

        _randomMock.Setup(r => r.Next(1))
            .Returns(0); // Selecting SkillAttack

        _characterLookupServiceMock.Setup(c => c.FindCharactersByIds(new List<int> { 1, 2 })).ReturnsAsync(characters);

        var DamageDealt = 10;
        _attackPerformServiceMock.Setup(a =>
                a.ExecuteAttack(It.IsAny<Character>(), It.IsAny<Character>(),
                    It.IsAny<Func<Character, Character, int>>()))
            .ReturnsAsync(new AttackResultDto { DamageDealt = DamageDealt });

        //Act
        await _fightService.Fight(fightRequestDto);

        //Assert
        FightServiceTestHelper.AssertLog(_fightLoggerMock, characters, AttackType.Skill, Times.Once(), DamageDealt);
    }


    [Fact]
    public async Task Fight_LogsSkipAttack_WhenCharacterHasNoWeaponAndNoSkills()
    {
        // Arange
        var fightRequestDto = new FightRequestDto
        {
            CharacterIds = new List<int> { 1, 2 }
        };

        var characters = new List<Character>
        {
            new() { Id = 1, HitPoints = 100, Weapon = null, Skills = new List<Skill>() },
            new() { Id = 2, HitPoints = 100, Weapon = null, Skills = new List<Skill>() }
        };

        _randomMock.SetupSequence(r => r.Next(characters.Count - 1))
            .Returns(0)
            .Throws(new Exception(
                "LoopTerminationForTestException"));

        _characterLookupServiceMock.Setup(c => c.FindCharactersByIds(new List<int> { 1, 2 })).ReturnsAsync(characters);

        //Act
        await _fightService.Fight(fightRequestDto);

        //Assert
        FightServiceTestHelper.AssertLog(_fightLoggerMock, characters, AttackType.Skip, Times.Once());
    }
}