using dotnet_rpg.Controllers;
using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Services.SkillService;
using static dotnet_rpg.Tests.TestHelper;

namespace dotnet_rpg.Tests.Controllers;

public class SkillControllerTests
{
    private readonly SkillController controller;
    private readonly Mock<ISkillService> mockService;

    public SkillControllerTests()
    {
        mockService = new Mock<ISkillService>();
        controller = new SkillController(mockService.Object);
    }

    [Fact]
    public async Task AddSkillToCharacter_ForMismatchedCharacterIds_ReturnsBadRequest()
    {
        // Arrange
        var characterId = 1;
        var addCharacterSkillDto = new AddCharacterSkillDto { CharacterId = 2 };

        // Act
        var result = await controller.AddSkillToCharacter(characterId, addCharacterSkillDto);

        //Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task AddSkillToCharacter_ForFailedServiceResponse_ReturnsBadRequest()
    {
        // Arrange
        var characterId = 1;
        var addCharacterSkillDto = new AddCharacterSkillDto { CharacterId = characterId };
        var expectedFailedServiceResponse =
            CreateServiceResponse(new GetCharacterResponseDto(), false, "Invalid skill id provided");

        mockService.SetupMockServiceCall(service => service.AddSkillToCharacter(addCharacterSkillDto),
            expectedFailedServiceResponse);

        // Act
        var result = await controller.AddSkillToCharacter(characterId, addCharacterSkillDto);

        //Assert
        CheckResponse(result, typeof(BadRequestObjectResult), expectedFailedServiceResponse);
    }

    [Fact]
    public async Task AddSkillToCharacter_WhenAddingASkillToACharacter_ReturnsGetCharacterResponseDto()
    {
        //Arrange 
        var characterId = 1;
        var addCharacterSkillDto = new AddCharacterSkillDto { CharacterId = characterId };
        var character = new GetCharacterResponseDto { Id = characterId, Name = "John" };
        var expectedServiceResponse = CreateServiceResponse(character);

        mockService.SetupMockServiceCall(service => service.AddSkillToCharacter(addCharacterSkillDto),
            expectedServiceResponse);

        //Act
        var result = await controller.AddSkillToCharacter(characterId, addCharacterSkillDto);

        //Assert
        CheckResponse(result, typeof(OkObjectResult), expectedServiceResponse);
    }
}