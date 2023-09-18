using dotnet_rpg.Dtos.Fight;
using dotnet_rpg.Repository;
using dotnet_rpg.Services.AttackPerformService;
using dotnet_rpg.Services.CharacterLookupService;

namespace dotnet_rpg.Tests.Services;

public class AttackPerformServiceTests
{
    private readonly Mock<ICharacterLookupService> _characterLookupServiceMock = new();
    private readonly Mock<IRepository<Character>> _characterRepositoryMock = new();
    private readonly AttackPerformService _service;

    public AttackPerformServiceTests()
    {
        _service = new AttackPerformService(_characterLookupServiceMock.Object, _characterRepositoryMock.Object);
    }

    [Fact]
    public async Task PerformAttack_ShouldReturnDefeatedMessage_WhenOpponentIsAlreadyDefeated()
    {
        // Arrange
        var attackDto = new IAttackDto { AttackerId = 1, OpponentId = 2 };
        var defeatedOpponent = new Character { Name = "DefeatedOpponent", HitPoints = 0 };
        _characterLookupServiceMock.Setup(x => x.FindCharacterByCharacterId(It.IsAny<int>()))
            .ReturnsAsync(defeatedOpponent);

        // Act
        var result = await _service.PerformAttack(attackDto, (a, o) => 10);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be($"{defeatedOpponent.Name} has already been defeated");
    }

    [Fact]
    public async Task PerformAttack_ShouldReturnDefeatedMessage_WhenOpponentGetsDefeatedAfterAttack()
    {
        // Arrange
        var attackDto = new IAttackDto { AttackerId = 1, OpponentId = 2 };
        var attacker = new Character { Name = "Attacker", HitPoints = 100 };
        var opponent = new Character { Name = "Opponent", HitPoints = 5 };

        _characterLookupServiceMock.Setup(x => x.FindCharacterByUserAndCharacterId(It.IsAny<int>()))
            .ReturnsAsync(attacker);
        _characterLookupServiceMock.Setup(x => x.FindCharacterByCharacterId(It.IsAny<int>())).ReturnsAsync(opponent);

        // Act
        var result = await _service.PerformAttack(attackDto, (a, o) =>
        {
            opponent.HitPoints = 0;
            return 10;
        });

        // Assert
        result.Success.Should().BeTrue();
        result.Message.Should().Be($"{opponent.Name} has been defeated");
    }

    [Fact]
    public async Task PerformAttack_ShouldReturnErrorMessage_WhenExceptionOccurs()
    {
        // Arrange
        var attackDto = new IAttackDto { AttackerId = 1, OpponentId = 2 };
        _characterLookupServiceMock.Setup(x => x.FindCharacterByUserAndCharacterId(It.IsAny<int>()))
            .Throws(new Exception("Test Exception"));

        // Act
        var result = await _service.PerformAttack(attackDto, (a, o) => 10);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Test Exception");
    }

    [Fact]
    public async Task PerformAttack_ShouldApplyAttackStrategyCorrectly()
    {
        // Arrange
        var attackDto = new IAttackDto { AttackerId = 1, OpponentId = 2 };
        var attacker = new Character { Name = "Attacker", HitPoints = 100 };
        var opponent = new Character { Name = "Opponent", HitPoints = 50 };

        _characterLookupServiceMock.Setup(x => x.FindCharacterByUserAndCharacterId(It.IsAny<int>()))
            .ReturnsAsync(attacker);
        _characterLookupServiceMock.Setup(x => x.FindCharacterByCharacterId(It.IsAny<int>())).ReturnsAsync(opponent);

        // Act
        var result = await _service.PerformAttack(attackDto, (a, o) => a.HitPoints - o.HitPoints);

        // Assert
        result.Data.DamageDealt.Should().Be(50);
    }

    [Fact]
    public async Task PerformAttack_ShouldUpdateFightStatistics()
    {
        // Arrange
        var attackDto = new IAttackDto { AttackerId = 1, OpponentId = 2 };
        var attacker = new Character { Name = "Attacker", HitPoints = 100, Fights = 5, Victories = 3 };
        var opponent = new Character { Name = "Opponent", HitPoints = 50, Fights = 10, Defeats = 2 };

        _characterLookupServiceMock.Setup(x => x.FindCharacterByUserAndCharacterId(It.IsAny<int>()))
            .ReturnsAsync(attacker);
        _characterLookupServiceMock.Setup(x => x.FindCharacterByCharacterId(It.IsAny<int>())).ReturnsAsync(opponent);

        // Act
        await _service.PerformAttack(attackDto, (a, o) =>
        {
            opponent.HitPoints = 0;
            return 50;
        });

        // Assert
        attacker.Fights.Should().Be(6);
        opponent.Fights.Should().Be(11);
        attacker.Victories.Should().Be(4);
        opponent.Defeats.Should().Be(3);
    }

    [Fact]
    public async Task PerformAttack_ShouldReturnCorrectAttackResultDto()
    {
        // Arrange
        var attackDto = new IAttackDto { AttackerId = 1, OpponentId = 2 };
        var attacker = new Character { Name = "Attacker", HitPoints = 100 };
        var opponent = new Character { Name = "Opponent", HitPoints = 50 };

        _characterLookupServiceMock.Setup(x => x.FindCharacterByUserAndCharacterId(It.IsAny<int>()))
            .ReturnsAsync(attacker);
        _characterLookupServiceMock.Setup(x => x.FindCharacterByCharacterId(It.IsAny<int>())).ReturnsAsync(opponent);

        // Act
        var result = await _service.PerformAttack(attackDto, (a, o) => 10);

        // Assert
        result.Data.Attacker.Should().Be(attacker.Name);
        result.Data.Opponent.Should().Be(opponent.Name);
        result.Data.AttackerHp.Should().Be(attacker.HitPoints);
        result.Data.OpponentHp.Should().Be(opponent.HitPoints);
        result.Data.DamageDealt.Should().Be(10);
    }
}