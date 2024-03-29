using AutoMapper;
using dotnet_rpg.Auth;
using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Exceptions;
using dotnet_rpg.Repository;
using dotnet_rpg.Services.CharacterLookupService;
using dotnet_rpg.Services.CharacterService;

namespace dotnet_rpg.Tests.Services;

public class CharacterServiceTests
{
    private readonly Mock<IAuthRepository> _authRepositoryMock;
    private readonly Mock<ICharacterLookupService> _characterLookupServiceMock;
    private readonly Mock<IRepository<Character>> _characterRepositoryMock;
    private readonly CharacterService _characterService;
    private readonly Mock<IMapper> _mapperMock;

    public CharacterServiceTests()
    {
        _authRepositoryMock = new Mock<IAuthRepository>();
        _characterRepositoryMock = new Mock<IRepository<Character>>();
        _mapperMock = new Mock<IMapper>();
        _characterLookupServiceMock = new Mock<ICharacterLookupService>();
        _characterService = new CharacterService(_mapperMock.Object, _characterRepositoryMock.Object,
            _authRepositoryMock.Object, _characterLookupServiceMock.Object);
    }

    [Fact]
    public async Task CreateCharacter_ShouldCreateCharacterAndReturnAll()
    {
        // Arrange
        var newCharacter = new AddCharacterRequestDto();
        var character = new Character();
        var user = new User();
        var currentUserId = 1;
        var characters = new List<Character>();
        var getCharacterResponseDtoList = new List<GetCharacterResponseDto>();

        _mapperMock.Setup(x => x.Map<Character>(newCharacter)).Returns(character);
        _authRepositoryMock.Setup(x => x.GetCurrentUserId()).Returns(currentUserId);
        _authRepositoryMock.Setup(x => x.GetByIdAsync(currentUserId)).ReturnsAsync(user);
        _characterRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(characters);
        _mapperMock.Setup(x => x.Map<List<GetCharacterResponseDto>>(characters)).Returns(getCharacterResponseDtoList);

        // Act
        var result = await _characterService.CreateCharacter(newCharacter);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().BeSameAs(getCharacterResponseDtoList);
        character.User.Should().Be(user);
        _characterRepositoryMock.Verify(x => x.Add(character), Times.Once);
        _characterRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetCharacterById_WhenLookupServiceFindsCharacter_ShouldReturnCharacter()
    {
        // Arrange
        var characterId = 1;
        var currentUserId = 1;
        var character = new Character { UserId = currentUserId };
        var getCharacterResponseDto = new GetCharacterResponseDto();

        _characterLookupServiceMock.Setup(x => x.FindCharacterByUserAndCharacterId(characterId))
            .ReturnsAsync(character);
        _characterRepositoryMock.Setup(x => x.GetByIdAsync(characterId)).ReturnsAsync(character);
        _mapperMock.Setup(x => x.Map<GetCharacterResponseDto>(character)).Returns(getCharacterResponseDto);

        // Act
        var result = await _characterService.GetCharacterById(characterId);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().Be(getCharacterResponseDto);
    }

    [Fact]
    public async Task GetCharacterById_WhenCharacterLookupFails_ShouldReturnError()
    {
        // Arrange
        var characterId = 1;
        var characterUserId = 2; // character's user id is not the same as the current user id
        var currentUserId = 1;
        var character = new Character { UserId = characterUserId };

        _characterLookupServiceMock.Setup(x => x.FindCharacterByUserAndCharacterId(characterId))
            .ThrowsAsync(new CharacterNotFoundException(characterId));
        _characterRepositoryMock.Setup(x => x.GetByIdAsync(characterId)).ReturnsAsync(character);

        // Act
        var result = await _characterService.GetCharacterById(characterId);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be($"Character with id: {characterId} not found.");
    }

    [Fact]
    public async Task GetAllCharacters_ShouldReturnAllCharacters()
    {
        // Arrange
        var characters = new List<Character>();
        var getCharacterResponseDtoList = new List<GetCharacterResponseDto>();

        _characterRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(characters);
        _mapperMock.Setup(x => x.Map<List<GetCharacterResponseDto>>(characters)).Returns(getCharacterResponseDtoList);

        // Act
        var result = await _characterService.GetAllCharacters();

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().BeSameAs(getCharacterResponseDtoList);
    }

    [Fact]
    public async Task UpdateCharacter_WhenRetrievedAndUpdated_ShouldReturnUpdatedCharacterAndSuccess()
    {
        // Arrange
        var characterId = 1;
        var updatedCharacter = new UpdateCharacterRequestDto { Id = characterId };
        var character = new Character { UserId = 1 };
        var currentUserId = 1;
        var getCharacterResponseDto = new GetCharacterResponseDto();

        _characterLookupServiceMock.Setup(x => x.FindCharacterByUserAndCharacterId(characterId))
            .ReturnsAsync(character);
        _characterRepositoryMock.Setup(x => x.GetByIdAsync(characterId)).ReturnsAsync(character);
        _mapperMock.Setup(x => x.Map<GetCharacterResponseDto>(character)).Returns(getCharacterResponseDto);

        // Act
        var result = await _characterService.UpdateCharacter(updatedCharacter);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().Be(getCharacterResponseDto);
        _characterRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateCharacter_WhenCharacterLookupFails_ShouldReturnError()
    {
        // Arrange
        var characterId = 1;
        var updatedCharacter = new UpdateCharacterRequestDto { Id = characterId };

        _characterLookupServiceMock.Setup(x => x.FindCharacterByUserAndCharacterId(characterId))
            .ThrowsAsync(new CharacterNotFoundException(characterId));

        // Act
        var result = await _characterService.UpdateCharacter(updatedCharacter);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be($"Character with id: {characterId} not found.");
    }

    [Fact]
    public async Task
        DeleteCharacter_WhenCharacterExists_ShouldDeleteCharacterAndReturnAllRemaining()
    {
        // Arrange
        var characterId = 1;
        var currentUserId = 1;
        var character = new Character { UserId = currentUserId };
        var characters = new List<Character>();
        var getCharacterResponseDtoList = new List<GetCharacterResponseDto>();

        _characterLookupServiceMock.Setup(x => x.FindCharacterByUserAndCharacterId(characterId))
            .ReturnsAsync(character);
        _characterRepositoryMock.Setup(x => x.GetByIdAsync(characterId)).ReturnsAsync(character);
        _characterRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(characters);
        _mapperMock.Setup(x => x.Map<List<GetCharacterResponseDto>>(characters)).Returns(getCharacterResponseDtoList);

        // Act
        var result = await _characterService.DeleteCharacter(characterId);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().BeSameAs(getCharacterResponseDtoList);
        _characterRepositoryMock.Verify(x => x.Delete(character), Times.Once);
        _characterRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteCharacter_WhenCharacterLookupFails_ShouldReturnError()
    {
        // Arrange
        var characterId = 1;

        _characterLookupServiceMock.Setup(x => x.FindCharacterByUserAndCharacterId(characterId))
            .ThrowsAsync(new CharacterNotFoundException(characterId));

        // Act
        var result = await _characterService.DeleteCharacter(characterId);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be($"Character with id: {characterId} not found.");
    }
}