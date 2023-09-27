using dotnet_rpg.Dtos.Fight;
using dotnet_rpg.Repository;
using dotnet_rpg.Services.AttackPerformService;
using dotnet_rpg.Services.AttackService;
using dotnet_rpg.Services.CharacterLookupService;
using dotnet_rpg.Services.FightLogger;
using dotnet_rpg.Services.FightService;
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
        var weaponAttackDto = new WeaponAttackDto();
        var expectedAttackResult = new ServiceResponse<AttackResultDto>();

        _attackPerformServiceMock.Setup(a => a.PerformAttack(
                weaponAttackDto, It.IsAny<Func<Character, Character, int>>()))
            .ReturnsAsync(expectedAttackResult);

        var result = await _fightService.WeaponAttack(weaponAttackDto);

        result.Should().BeEquivalentTo(expectedAttackResult);
    }

    [Fact]
    public async Task SkillAttack_PerformsSkillAttack()
    {
        var skillAttackDto = new SkillAttackDto { SkillId = 123 };
        var expectedAttackResult = new ServiceResponse<AttackResultDto>();

        _attackPerformServiceMock.Setup(a => a.PerformAttack(
                skillAttackDto, It.IsAny<Func<Character, Character, int>>()))
            .ReturnsAsync(expectedAttackResult);

        var result = await _fightService.SkillAttack(skillAttackDto);

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
        var fightRequestDto = new FightRequestDto
        {
            CharacterIds = new List<int> { 1, 2 }
        };

        var (characters, expectedAttackResult) = SetupForWeaponAttack();

        _randomMock.SetupSequence(r => r.Next(It.IsAny<int>()))
            .Returns(0) // For selecting AttackType.Weapon
            .Returns(0); // For selecting the opponent

        await _fightService.Fight(fightRequestDto);

        // Specific assertions for `FightResultDto` isn't necessary as this is just a string of logs that is passed in
        _attackServiceMock.Verify(a => a.DoWeaponAttack(It.IsAny<Character>(), It.IsAny<Character>()), Times.Once);
        _fightLoggerMock.Verify(
            l => l.LogAttack(characters[0], characters[1], expectedAttackResult.DamageDealt, AttackType.Weapon,
                It.IsAny<FightResultDto>()), Times.Once);
        _fightLoggerMock.Verify(
            l => l.LogVictory(characters[0], characters[1], It.IsAny<FightResultDto>()), Times.Once);
    }



    private (List<Character>, AttackResultDto)  SetupForWeaponAttack(int opponentHP = 5, int damageDealt = 5)
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
}