using dotnet_rpg.Controllers;
using dotnet_rpg.Dtos.Fight;
using dotnet_rpg.Services.HighScoreService;
using static dotnet_rpg.Tests.TestHelper;

namespace dotnet_rpg.Tests.Controllers;

public class ScoreControllerTests
{
    private readonly ScoreController _controller;
    private readonly Mock<IHighScoreService> _mockHighScoreService;

    public ScoreControllerTests()
    {
        _mockHighScoreService = new Mock<IHighScoreService>();
        _controller = new ScoreController(_mockHighScoreService.Object);
    }

    [Fact]
    public async Task GetCharacterScores_WhenRetrievingCharacterScores_ReturnsListOfGetHighScoreDto()
    {
        // Arrange 
        var highScoreList = new List<GetHighScoreDto>
        {
            new() { Id = 1, Name = "John", Fights = 10, Victories = 5, Defeats = 5 },
            new() { Id = 2, Name = "Jane", Fights = 10, Victories = 5, Defeats = 5 }
        };
        var expectedServiceResponse = CreateServiceResponse(highScoreList);
        _mockHighScoreService.SetupMockServiceCall(service => service.GetCharactersByScore(), expectedServiceResponse);

        // Act
        var result = await _controller.GetCharacterScores();

        // Assert
        CheckResponse(result, typeof(OkObjectResult), expectedServiceResponse);
    }

    [Fact]
    public async Task GetCharacterScores_ForFailedServiceResponse_ReturnsBadRequest()
    {
        // Arrange
        var expectedFailedServiceResponse =
            CreateServiceResponse<List<GetHighScoreDto>>(null, false, "Failed to retrieve character scores");
        _mockHighScoreService.SetupMockServiceCall(service => service.GetCharactersByScore(),
            expectedFailedServiceResponse);

        // Act
        var result = await _controller.GetCharacterScores();

        // Assert
        CheckResponse(result, typeof(BadRequestObjectResult), expectedFailedServiceResponse);
    }
}