using dotnet_rpg.Auth;
using dotnet_rpg.Exceptions;
using dotnet_rpg.Repository;
using dotnet_rpg.Services.CharacterLookupService;

namespace dotnet_rpg.Tests.Services;

public class CharacterLookupServiceTests
{
    [Fact]
    public async Task FindCharacterByUserAndCharacterId_ReturnsCharacter_WhenCharacterExistsAndUserIdMatches()
    {
        // Arrange
        var mockAuthRepository = new Mock<IAuthRepository>();
        var mockCharacterRepository = new Mock<IRepository<Character>>();
        var service = new CharacterLookupService(mockCharacterRepository.Object, mockAuthRepository.Object);

        var character = new Character { Id = 1, UserId = 2 };
        mockAuthRepository.Setup(x => x.GetCurrentUserId()).Returns(2);
        mockCharacterRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(character);

        // Act
        var result = await service.FindCharacterByUserAndCharacterId(1);

        // Assert
        result.Should().Be(character);
    }

    [Fact]
    public async Task FindCharacterByUserAndCharacterId_ThrowsException_WhenCharacterNotFound()
    {
        // Arrange
        var mockAuthRepository = new Mock<IAuthRepository>();
        var mockCharacterRepository = new Mock<IRepository<Character>>();
        var service = new CharacterLookupService(mockCharacterRepository.Object, mockAuthRepository.Object);

        mockAuthRepository.Setup(x => x.GetCurrentUserId()).Returns(2);
        mockCharacterRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Character)null);

        // Act
        Func<Task> func = async () => await service.FindCharacterByUserAndCharacterId(1);

        // Assert
        await func.Should().ThrowAsync<CharacterNotFoundException>();
    }

    [Fact]
    public async Task FindCharacterByUserAndCharacterId_ThrowsException_WhenUserIdDoesNotMatch()
    {
        // Arrange
        var mockAuthRepository = new Mock<IAuthRepository>();
        var mockCharacterRepository = new Mock<IRepository<Character>>();
        var service = new CharacterLookupService(mockCharacterRepository.Object, mockAuthRepository.Object);

        var character = new Character { Id = 1, UserId = 3 };
        mockAuthRepository.Setup(x => x.GetCurrentUserId()).Returns(2);
        mockCharacterRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(character);

        // Act
        Func<Task> func = async () => await service.FindCharacterByUserAndCharacterId(1);

        // Assert
        await func.Should().ThrowAsync<CharacterNotFoundException>();
    }
}