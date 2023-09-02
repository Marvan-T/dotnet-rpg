using dotnet_rpg.Controllers;
using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Services.CharacterService;

namespace dotnet_rpg.Tests.Controllers;

public class CharacterControllerTest
{
    private readonly CharacterController controller;
    private readonly Mock<ICharacterService> mockService;

    public CharacterControllerTest()
    {
        mockService = new Mock<ICharacterService>();
        controller = new CharacterController(mockService.Object);
    }

    [Fact]
    public async Task Get_WhenRetrievingCharacters_ReturnsListOfGetCharacterResponseDto()
    {
        //Arrange 
        var characterList = new List<GetCharacterResponseDto>
        {
            new() { Id = 1, Name = "John" },
            new() { Id = 2, Name = "Jane" }
        };

        var serviceResponse = TestHelper.CreateServiceResponse(characterList);

        //Extension method where we call static method as an instance method. This works because we are calling this mockservice
        //in the implementation of SetupMockServiceCall
        mockService.SetupMockServiceCall(service => service.GetAllCharacters(), serviceResponse);

        //Act
        var result = await controller.Get();

        //Assert
        TestHelper.CheckResponse(result, typeof(OkObjectResult), serviceResponse);
    }

    [Fact]
    public async Task GetCharacterById_WhenRetrievingCharacterById_ReturnsGetCharacterResponseDto()
    {
        //Arrange 
        var characterId = 1;
        var character = new GetCharacterResponseDto { Id = characterId, Name = "John" };
        var serviceResponse = TestHelper.CreateServiceResponse(character);
        mockService.SetupMockServiceCall(service => service.GetCharacterById(characterId), serviceResponse);

        //Act
        var result = await controller.GetCharacterById(characterId);

        //Assert
        TestHelper.CheckResponse(result, typeof(OkObjectResult), serviceResponse);
    }

    [Fact]
    public async Task CreateCharacter_WhenCreatingANewCharacter_ReturnsListOfGetCharacterResponseDto()
    {
        //Arrange 
        var newCharacter = new AddCharacterRequestDto { Name = "Jane" };
        var characterList = new List<GetCharacterResponseDto>
        {
            new() { Id = 1, Name = "John" },
            new() { Id = 2, Name = "Jane" }
        };

        var serviceResponse = TestHelper.CreateServiceResponse(characterList);
        mockService.SetupMockServiceCall(service => service.CreateCharacter(newCharacter), serviceResponse);

        //Act
        var result = await controller.CreateCharacter(newCharacter);

        //Assert
        TestHelper.CheckResponse(result, typeof(OkObjectResult), serviceResponse);
    }

    [Fact]
    public async Task UpdateCharacter_ForFailedServiceResponse_ReturnsNotFound()
    {
        // Arrange
        var updatedCharacter = new UpdateCharacterRequestDto();
        var failedServiceResponse =
            TestHelper.CreateServiceResponse<GetCharacterResponseDto>(null, false, "Invalid character id provided");
        mockService.SetupMockServiceCall(service => service.UpdateCharacter(updatedCharacter), failedServiceResponse);

        // Act
        var result = await controller.UpdateCharacter(updatedCharacter);

        //Assert
        TestHelper.CheckResponse(result, typeof(NotFoundObjectResult), failedServiceResponse);
    }

    [Fact]
    public async Task UpdateCharacter_WhenUpdatingACharacter_ReturnsGetCharacterResponseDto()
    {
        //Arrange 
        var characterId = 1;
        var updatedCharacter = new UpdateCharacterRequestDto { Id = characterId, Name = "Jane" };
        var character = new GetCharacterResponseDto { Id = characterId, Name = "Jane" };
        var serviceResponse = TestHelper.CreateServiceResponse(character);
        mockService.SetupMockServiceCall(service => service.UpdateCharacter(updatedCharacter), serviceResponse);

        //Act
        var result = await controller.UpdateCharacter(updatedCharacter);

        //Assert
        TestHelper.CheckResponse(result, typeof(OkObjectResult), serviceResponse);
    }

    [Fact]
    public async Task DeleteCharacter_ForFailedServiceResponse_ReturnsNotFound()
    {
        // Arrange
        var characterId = 1;
        var failedServiceResponse =
            TestHelper.CreateServiceResponse(new List<GetCharacterResponseDto>(), false,
                "Invalid character id provided");
        mockService.SetupMockServiceCall(service => service.DeleteCharacter(characterId), failedServiceResponse);

        // Act
        var result = await controller.DeleteCharacter(characterId);

        //Assert
        TestHelper.CheckResponse(result, typeof(NotFoundObjectResult), failedServiceResponse);
    }

    [Fact]
    public async Task DeleteCharacter_WhenDeletingACharacter_ReturnsListOfGetCharacterResponseDto()
    {
        //Arrange 
        var characterId = 1;
        var characterList = new List<GetCharacterResponseDto>
        {
            new() { Id = 2, Name = "Jane" }
        };
        var serviceResponse = TestHelper.CreateServiceResponse(characterList);
        mockService.SetupMockServiceCall(service => service.DeleteCharacter(characterId), serviceResponse);

        //Act
        var result = await controller.DeleteCharacter(characterId);

        //Assert
        TestHelper.CheckResponse(result, typeof(OkObjectResult), serviceResponse);
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
        var failedServiceResponse =
            TestHelper.CreateServiceResponse(new GetCharacterResponseDto(), false, "Invalid skill id provided");

        mockService.SetupMockServiceCall(service => service.AddSkillToCharacter(addCharacterSkillDto),
            failedServiceResponse);

        // Act
        var result = await controller.AddSkillToCharacter(characterId, addCharacterSkillDto);

        //Assert
        TestHelper.CheckResponse(result, typeof(BadRequestObjectResult), failedServiceResponse);
    }

    [Fact]
    public async Task AddSkillToCharacter_WhenAddingASkillToACharacter_ReturnsGetCharacterResponseDto()
    {
        //Arrange 
        var characterId = 1;
        var addCharacterSkillDto = new AddCharacterSkillDto { CharacterId = characterId };
        var character = new GetCharacterResponseDto { Id = characterId, Name = "John" };
        var serviceResponse = TestHelper.CreateServiceResponse(character);

        mockService.SetupMockServiceCall(service => service.AddSkillToCharacter(addCharacterSkillDto), serviceResponse);

        //Act
        var result = await controller.AddSkillToCharacter(characterId, addCharacterSkillDto);

        //Assert
        TestHelper.CheckResponse(result, typeof(OkObjectResult), serviceResponse);
    }
}