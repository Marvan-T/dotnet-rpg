using AutoMapper;
using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Repository;
using dotnet_rpg.Services.CharacterLookupService;
using dotnet_rpg.Services.SkillService;

namespace dotnet_rpg.Tests.Services;

public class SkillServiceTests
{
    private readonly Mock<ICharacterLookupService> _chaterLookupServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepository<Skill>> _skillRepositoryMock;
    private readonly SkillService _skillService;

    public SkillServiceTests()
    {
        _mapperMock = new Mock<IMapper>();
        _skillRepositoryMock = new Mock<IRepository<Skill>>();
        _chaterLookupServiceMock = new Mock<ICharacterLookupService>();
        _skillService = new SkillService(
            _skillRepositoryMock.Object,
            _chaterLookupServiceMock.Object,
            _mapperMock.Object
        );
    }


    [Fact]
    public async Task AddSkill_ExistingCharacterAndSkill_ShouldUpdateAndReturnCharacter()
    {
        // Arrange
        var characterId = 1;
        var skillId = 1;
        var addCharacterSkillDto = new AddCharacterSkillDto { CharacterId = characterId, SkillId = skillId };
        var character = new Character { Skills = new List<Skill>() };
        var skill = new Skill();
        var getCharacterResponseDto = new GetCharacterResponseDto();

        _chaterLookupServiceMock.Setup(x => x.FindCharacterByUserAndCharacterId(characterId)).ReturnsAsync(character);
        _skillRepositoryMock.Setup(x => x.GetByIdAsync(skillId)).ReturnsAsync(skill);
        _mapperMock.Setup(x => x.Map<GetCharacterResponseDto>(character)).Returns(getCharacterResponseDto);

        // Act
        var result = await _skillService.AddSkillToCharacter(addCharacterSkillDto);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().Be(getCharacterResponseDto);
        character.Skills.Should().Contain(skill);
        _skillRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AddSkillToCharacter_WhenSkillDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var characterId = 1;
        var skillId = 1;
        var addCharacterSkillDto = new AddCharacterSkillDto { CharacterId = characterId, SkillId = skillId };
        var character = new Character();

        _chaterLookupServiceMock.Setup(x => x.FindCharacterByUserAndCharacterId(characterId)).ReturnsAsync(character);
        _skillRepositoryMock.Setup(x => x.GetByIdAsync(skillId)).ReturnsAsync((Skill)null);

        // Act
        var result = await _skillService.AddSkillToCharacter(addCharacterSkillDto);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be($"Skill with id: {skillId} not found.");
    }

    [Fact]
    public async Task AddSkillToCharacter_WhenSkillIsAlreadyAddedToCharacter_ShouldReturnError()
    {
        // Arrange
        var characterId = 1;
        var skillId = 1;
        var addCharacterSkillDto = new AddCharacterSkillDto { CharacterId = characterId, SkillId = skillId };
        var character = new Character { Skills = new List<Skill> { new() { Id = skillId } } };
        var skill = new Skill { Id = skillId };

        _chaterLookupServiceMock.Setup(x => x.FindCharacterByUserAndCharacterId(characterId)).ReturnsAsync(character);
        _skillRepositoryMock.Setup(x => x.GetByIdAsync(skillId)).ReturnsAsync(skill);

        // Act
        var result = await _skillService.AddSkillToCharacter(addCharacterSkillDto);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be($"Skill with ID {skillId} is already added to Character with ID {character.Id}");
    }
}