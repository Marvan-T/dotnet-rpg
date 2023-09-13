using dotnet_rpg.Dtos.Fight;
using dotnet_rpg.Repository;
using dotnet_rpg.Services.CharacterLookupService;
using dotnet_rpg.Services.FightService;
using dotnet_rpg.Utility.RandomGeneration;

namespace dotnet_rpg.Tests.Services;

public class FightServiceTests
{
    private readonly Mock<ICharacterLookupService> _characterLookupServiceMock;
    private readonly Mock<IRepository<Character>> _characterRepositoryMock;
    private readonly FightService _fightService;
    private readonly Mock<IRandomGenerator> _randomGeneratorMock;

    public FightServiceTests()
    {
        _characterLookupServiceMock = new Mock<ICharacterLookupService>();
        _characterRepositoryMock = new Mock<IRepository<Character>>();
        _randomGeneratorMock = new Mock<IRandomGenerator>();
        _fightService = new FightService(_characterLookupServiceMock.Object, _characterRepositoryMock.Object,
            _randomGeneratorMock.Object);
    }

    [Fact]
    public async Task WeaponAttack_OpponentAlreadyDefeated_ReturnsFailureResponse()
    {
        // Arrange
        var weaponAttackDto = new WeaponAttackDto { AttackerId = 1, OpponentId = 2 };
        var defeatedOpponent = new Character { HitPoints = 0, Name = "DefeatedOpponent" };
        _characterLookupServiceMock.Setup(s => s.FindCharacterByUserAndCharacterId(weaponAttackDto.AttackerId))
            .ReturnsAsync(new Character());
        _characterLookupServiceMock.Setup(s => s.FindCharacterByCharacterId(weaponAttackDto.OpponentId))
            .ReturnsAsync(defeatedOpponent);

        // Act
        var result = await _fightService.WeaponAttack(weaponAttackDto);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be($"{defeatedOpponent.Name} has already been defeated");
    }

    [Fact]
    public async Task WeaponAttack_AttackerHasNoWeapon_ReturnsFailureResponse()
    {
        // Arrange
        var weaponAttackDto = new WeaponAttackDto { AttackerId = 1, OpponentId = 2 };
        var attacker = new Character { Id = 1, Weapon = null };
        _characterLookupServiceMock.Setup(s => s.FindCharacterByUserAndCharacterId(weaponAttackDto.AttackerId))
            .ReturnsAsync(attacker);
        _characterLookupServiceMock.Setup(s => s.FindCharacterByCharacterId(weaponAttackDto.OpponentId))
            .ReturnsAsync(new Character());

        // Act
        var result = await _fightService.WeaponAttack(weaponAttackDto);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be($"Attacker with ID {attacker.Id} does not have a weapon.");
    }


    [Fact]
    public async Task WeaponAttack_ValidAttack_UpdatesFightStatisticsAndSavesChanges()
    {
        // Arrange
        var weaponAttackDto = new WeaponAttackDto { AttackerId = 1, OpponentId = 2 };
        var attacker = new Character { Id = 1, Fights = 0, Victories = 0, Weapon = new Weapon(), Strength = 10 };
        var opponent = new Character { Id = 2, Fights = 0, Defeats = 0, HitPoints = 20, Defense = 5 };
        _characterLookupServiceMock.Setup(s => s.FindCharacterByUserAndCharacterId(weaponAttackDto.AttackerId))
            .ReturnsAsync(attacker);
        _characterLookupServiceMock.Setup(s => s.FindCharacterByCharacterId(weaponAttackDto.OpponentId))
            .ReturnsAsync(opponent);
        _randomGeneratorMock.Setup(r => r.Next(It.IsAny<int>())).Returns(5);

        // Act
        var result = await _fightService.WeaponAttack(weaponAttackDto);

        // Assert
        result.Success.Should().BeTrue();
        attacker.Fights.Should().Be(1);
        opponent.Fights.Should().Be(1);
        _characterRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task WeaponAttack_ValidAttack_UpdatesVictoriesAndDefeats()
    {
        // Arrange
        var weaponAttackDto = new WeaponAttackDto { AttackerId = 1, OpponentId = 2 };
        var attacker = new Character
            { Id = 1, Fights = 0, Victories = 0, Weapon = new Weapon { Damage = 5 }, Strength = 10 };
        var opponent = new Character
        {
            Id = 2, Fights = 0, Defeats = 0, HitPoints = 5, Defense = 5
        }; // HitPoints set to a low value so that the opponent is defeated
        _characterLookupServiceMock.Setup(s => s.FindCharacterByUserAndCharacterId(weaponAttackDto.AttackerId))
            .ReturnsAsync(attacker);
        _characterLookupServiceMock.Setup(s => s.FindCharacterByCharacterId(weaponAttackDto.OpponentId))
            .ReturnsAsync(opponent);
        _randomGeneratorMock.Setup(r => r.Next(attacker.Strength)).Returns(5);

        // Act
        var result = await _fightService.WeaponAttack(weaponAttackDto);

        // Assert
        result.Success.Should().BeTrue();
        attacker.Victories.Should().Be(1);
        opponent.Defeats.Should().Be(1);
    }

    [Fact]
    public async Task WeaponAttack_ValidAttack_UpdatesOpponentHitPoints()
    {
        // Arrange
        var weaponAttackDto = new WeaponAttackDto { AttackerId = 1, OpponentId = 2 };
        var attacker = new Character { Id = 1, Fights = 0, Victories = 0, Weapon = new Weapon(), Strength = 10 };
        var opponent = new Character { Id = 2, Fights = 0, Defeats = 0, HitPoints = 20, Defense = 5 };
        _characterLookupServiceMock.Setup(s => s.FindCharacterByUserAndCharacterId(weaponAttackDto.AttackerId))
            .ReturnsAsync(attacker);
        _characterLookupServiceMock.Setup(s => s.FindCharacterByCharacterId(weaponAttackDto.OpponentId))
            .ReturnsAsync(opponent);
        _randomGeneratorMock.Setup(r => r.Next(attacker.Strength)).Returns(4);
        _randomGeneratorMock.Setup(r => r.Next(opponent.Defense)).Returns(2);

        // Act
        var result = await _fightService.WeaponAttack(weaponAttackDto);

        // Assert
        result.Success.Should().BeTrue();
        opponent.HitPoints.Should().Be(18); // 20 - (4 - 2)
        attacker.HitPoints.Should().Be(100);
    }
}