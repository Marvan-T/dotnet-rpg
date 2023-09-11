using dotnet_rpg.Controllers;
using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Services.CharacterService;
using static dotnet_rpg.Tests.TestHelper;

namespace dotnet_rpg.Tests.Controllers;

public class CharacterControllerTests
{
    private readonly CharacterController controller;
    private readonly Mock<ICharacterService> mockService;

    public CharacterControllerTests()
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

        var expectedServiceResponse = CreateServiceResponse(characterList);

        //Extension method where we call static method as an instance method. This works because we are calling this mockservice
        //in the implementation of SetupMockServiceCall
        mockService.SetupMockServiceCall(service => service.GetAllCharacters(), expectedServiceResponse);

        //Act
        var result = await controller.Get();

        //Assert
        CheckResponse(result, typeof(OkObjectResult), expectedServiceResponse);
    }

    [Fact]
    public async Task GetCharacterById_WhenRetrievingCharacterById_ReturnsGetCharacterResponseDto()
    {
        //Arrange 
        var characterId = 1;
        var character = new GetCharacterResponseDto { Id = characterId, Name = "John" };
        var expectedServiceResponse = CreateServiceResponse(character);
        mockService.SetupMockServiceCall(service => service.GetCharacterById(characterId), expectedServiceResponse);

        //Act
        var result = await controller.GetCharacterById(characterId);

        //Assert
        CheckResponse(result, typeof(OkObjectResult), expectedServiceResponse);
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

        var expectedServiceResponse = CreateServiceResponse(characterList);
        mockService.SetupMockServiceCall(service => service.CreateCharacter(newCharacter), expectedServiceResponse);

        //Act
        var result = await controller.CreateCharacter(newCharacter);

        //Assert
        CheckResponse(result, typeof(OkObjectResult), expectedServiceResponse);
    }

    [Fact]
    public async Task UpdateCharacter_ForFailedServiceResponse_ReturnsNotFound()
    {
        // Arrange
        var updatedCharacter = new UpdateCharacterRequestDto();
        var expectedFailedServiceResponse =
            CreateServiceResponse<GetCharacterResponseDto>(null, false, "Invalid character id provided");
        mockService.SetupMockServiceCall(service => service.UpdateCharacter(updatedCharacter),
            expectedFailedServiceResponse);

        // Act
        var result = await controller.UpdateCharacter(updatedCharacter);

        //Assert
        CheckResponse(result, typeof(NotFoundObjectResult), expectedFailedServiceResponse);
    }

    [Fact]
    public async Task UpdateCharacter_WhenUpdatingACharacter_ReturnsGetCharacterResponseDto()
    {
        //Arrange 
        var characterId = 1;
        var updatedCharacter = new UpdateCharacterRequestDto { Id = characterId, Name = "Jane" };
        var character = new GetCharacterResponseDto { Id = characterId, Name = "Jane" };
        var expectedServiceResponse = CreateServiceResponse(character);
        mockService.SetupMockServiceCall(service => service.UpdateCharacter(updatedCharacter), expectedServiceResponse);

        //Act
        var result = await controller.UpdateCharacter(updatedCharacter);

        //Assert
        CheckResponse(result, typeof(OkObjectResult), expectedServiceResponse);
    }

    [Fact]
    public async Task DeleteCharacter_ForFailedServiceResponse_ReturnsNotFound()
    {
        // Arrange
        var characterId = 1;
        var expectedFailedServiceResponse =
            CreateServiceResponse(new List<GetCharacterResponseDto>(), false,
                "Invalid character id provided");
        mockService.SetupMockServiceCall(service => service.DeleteCharacter(characterId),
            expectedFailedServiceResponse);

        // Act
        var result = await controller.DeleteCharacter(characterId);

        //Assert
        CheckResponse(result, typeof(NotFoundObjectResult), expectedFailedServiceResponse);
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
        var expectedServiceResponse = CreateServiceResponse(characterList);
        mockService.SetupMockServiceCall(service => service.DeleteCharacter(characterId), expectedServiceResponse);

        //Act
        var result = await controller.DeleteCharacter(characterId);

        //Assert
        CheckResponse(result, typeof(OkObjectResult), expectedServiceResponse);
    }
}