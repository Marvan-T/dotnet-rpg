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

        var serviceResponse = new ServiceResponse<List<GetCharacterResponseDto>>
        {
            Data = characterList
        };

        SetupMockService(serviceResponse);

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

        var serviceResponse = new ServiceResponse<GetCharacterResponseDto>
        {
            Data = character
        };

        mockService.Setup(service => service.GetCharacterById(characterId))
            .ReturnsAsync(serviceResponse);

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

        var serviceResponse = new ServiceResponse<List<GetCharacterResponseDto>>
        {
            Data = characterList
        };

        mockService.Setup(service => service.CreateCharacter(newCharacter))
            .ReturnsAsync(serviceResponse);

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
        var failedServiceResponse = new ServiceResponse<GetCharacterResponseDto>
        {
            Success = false,
            Message = "Invalid character id provided"
        };

        mockService.Setup(service => service.UpdateCharacter(updatedCharacter))
            .ReturnsAsync(failedServiceResponse);

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

        var serviceResponse = new ServiceResponse<GetCharacterResponseDto>
        {
            Data = character
        };

        mockService.Setup(service => service.UpdateCharacter(updatedCharacter))
            .ReturnsAsync(serviceResponse);

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

        var failedServiceResponse = new ServiceResponse<List<GetCharacterResponseDto>>
        {
            Success = false,
            Message = "Invalid character id provided"
        };

        mockService.Setup(service => service.DeleteCharacter(characterId))
            .ReturnsAsync(failedServiceResponse);

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

        var serviceResponse = new ServiceResponse<List<GetCharacterResponseDto>>
        {
            Data = characterList
        };

        mockService.Setup(service => service.DeleteCharacter(characterId))
            .ReturnsAsync(serviceResponse);

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
        
        var failedServiceResponse = new ServiceResponse<GetCharacterResponseDto>
        {
            Success = false,
            Message = "Invalid skill id provided"
        };

        mockService.Setup(service => service.AddSkillToCharacter(addCharacterSkillDto))
            .ReturnsAsync(failedServiceResponse);

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
        
        var serviceResponse = new ServiceResponse<GetCharacterResponseDto>
        {
            Data = character
        };

        mockService.Setup(service => service.AddSkillToCharacter(addCharacterSkillDto))
            .ReturnsAsync(serviceResponse);

        //Act
        var result = await controller.AddSkillToCharacter(characterId, addCharacterSkillDto);

        //Assert
        TestHelper.CheckResponse(result, typeof(OkObjectResult), serviceResponse);
    }

    private void SetupMockService(ServiceResponse<List<GetCharacterResponseDto>> expectedServiceResponse)
    {
        mockService.Setup(service => service.GetAllCharacters())
            .ReturnsAsync(expectedServiceResponse);
    }
}