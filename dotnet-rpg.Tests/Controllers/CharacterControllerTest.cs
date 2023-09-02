using System.Linq.Expressions;
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

    private void SetupService<T>(Expression<Func<ICharacterService, Task<ServiceResponse<T>>>> call,
        ServiceResponse<T> response)
    {
        mockService.Setup(call).ReturnsAsync(response);
    }

    private ServiceResponse<T> CreateServiceResponse<T>(T data, bool success = true, string message = null)
    {
        return new ServiceResponse<T> { Data = data, Success = success, Message = message };
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

        var serviceResponse = CreateServiceResponse(characterList);
        SetupService(service => service.GetAllCharacters(), serviceResponse);

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
        var serviceResponse = CreateServiceResponse(character);
        SetupService(service => service.GetCharacterById(characterId), serviceResponse);

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

        var serviceResponse = CreateServiceResponse(characterList);
        SetupService(service => service.CreateCharacter(newCharacter), serviceResponse);

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
            CreateServiceResponse<GetCharacterResponseDto>(null, false, "Invalid character id provided");
        SetupService(service => service.UpdateCharacter(updatedCharacter), failedServiceResponse);

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
        var serviceResponse = CreateServiceResponse(character);
        SetupService(service => service.UpdateCharacter(updatedCharacter), serviceResponse);

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
            CreateServiceResponse(new List<GetCharacterResponseDto>(), false, "Invalid character id provided");
        SetupService(service => service.DeleteCharacter(characterId), failedServiceResponse);

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
        var serviceResponse = CreateServiceResponse(characterList);
        SetupService(service => service.DeleteCharacter(characterId), serviceResponse);

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
            CreateServiceResponse(new GetCharacterResponseDto(), false, "Invalid skill id provided");

        SetupService(service => service.AddSkillToCharacter(addCharacterSkillDto), failedServiceResponse);

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
        var serviceResponse = CreateServiceResponse(character);

        SetupService(service => service.AddSkillToCharacter(addCharacterSkillDto), serviceResponse);

        //Act
        var result = await controller.AddSkillToCharacter(characterId, addCharacterSkillDto);

        //Assert
        TestHelper.CheckResponse(result, typeof(OkObjectResult), serviceResponse);
    }
}