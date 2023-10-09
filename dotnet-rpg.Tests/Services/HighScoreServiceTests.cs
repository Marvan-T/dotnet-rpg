using AutoMapper;
using dotnet_rpg.Dtos.Fight;
using dotnet_rpg.Repository;
using dotnet_rpg.Services.HighScoreService;
using dotnet_rpg.Specifications.CharacterSpecifications;

namespace dotnet_rpg.Tests.Services;

public class HighScoreServiceTests
{
    private readonly Mock<IRepository<Character>> _characterRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly HighScoreService _service;

    public HighScoreServiceTests()
    {
        _characterRepositoryMock = new Mock<IRepository<Character>>();
        _mapperMock = new Mock<IMapper>();
        _service = new HighScoreService(_characterRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetCharactersByScore_ReturnsMappedCharacters_WhenRepositoryReturnsCharacters()
    {
        // Arrange
        var characters = new List<Character>
        {
            new() { Id = 1, Name = "John" },
            new() { Id = 2, Name = "Jane" }
        };
        var dtos = new List<GetHighScoreDto>
        {
            new() { Id = 1, Name = "John" },
            new() { Id = 2, Name = "Jane" }
        };
        _characterRepositoryMock.Setup(repo => repo.ListAsync(It.IsAny<CharacterSortedByScoreSpecification>()))
            .ReturnsAsync(characters);
        _mapperMock.Setup(mapper => mapper.Map<List<GetHighScoreDto>>(characters)).Returns(dtos);

        // Act
        var result = await _service.GetCharactersByScore();

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().BeEquivalentTo(dtos);
    }

    [Fact]
    public async Task GetCharactersByScore_ReturnsFailure_WhenExceptionIsThrown()
    {
        // Arrange
        _characterRepositoryMock.Setup(repo => repo.ListAsync(It.IsAny<CharacterSortedByScoreSpecification>()))
            .ThrowsAsync(new Exception("Test Exception"));

        // Act
        var result = await _service.GetCharactersByScore();

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Test Exception");
    }
}