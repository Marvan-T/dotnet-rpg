using AutoMapper;
using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Dtos.Weapon;
using dotnet_rpg.Repository;
using dotnet_rpg.Services.WeaponService;

namespace dotnet_rpg.Tests.Services;

public class WeaponServiceTests
{
    private readonly Mock<IRepository<Character>> _characterRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepository<Weapon>> _weaponRepositoryMock;
    private readonly WeaponService _weaponService;

    public WeaponServiceTests()
    {
        _characterRepositoryMock = new Mock<IRepository<Character>>();
        _weaponRepositoryMock = new Mock<IRepository<Weapon>>();
        _mapperMock = new Mock<IMapper>();
        _weaponService = new WeaponService(_characterRepositoryMock.Object, _weaponRepositoryMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task AddWeaponToCharacter_WhenCharacterExists_ShouldAddWeaponToCharacter()
    {
        // Arrange
        var characterId = 1;
        var addWeaponDto = new AddWeaponDto { CharacterId = characterId };
        var character = new Character();
        var weapon = new Weapon();
        var getCharacterResponseDto = new GetCharacterResponseDto();

        _characterRepositoryMock.SetupMockMethodCall(x => x.GetByIdAsync(characterId), Task.FromResult(character));
        _mapperMock.SetupMockMethodCall(x => x.Map<Weapon>(addWeaponDto), weapon);
        _mapperMock.SetupMockMethodCall(x => x.Map<GetCharacterResponseDto>(character), getCharacterResponseDto);

        // Act
        var result = await _weaponService.AddWeaponToCharacter(addWeaponDto);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().Be(getCharacterResponseDto);
        _weaponRepositoryMock.Verify(x => x.Add(weapon), Times.Once);
        _weaponRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AddWeaponToCharacter_WhenCharacterDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var characterId = 1;
        var addWeaponDto = new AddWeaponDto { CharacterId = characterId };
        _characterRepositoryMock.SetupMockMethodCall(x => x.GetByIdAsync(characterId),
            Task.FromResult((Character)null));

        // Act
        var result = await _weaponService.AddWeaponToCharacter(addWeaponDto);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be($"Character with id: {characterId} not found.");
    }
}