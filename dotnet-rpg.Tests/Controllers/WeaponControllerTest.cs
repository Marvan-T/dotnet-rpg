using dotnet_rpg.Controllers;
using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Dtos.Weapon;
using dotnet_rpg.Services.WeaponService;

namespace dotnet_rpg.Tests.Controllers;

public class WeaponControllerTest
{
    private readonly WeaponController controller;
    private readonly Mock<IWeaponService> mockService;

    public WeaponControllerTest()
    {
        mockService = new Mock<IWeaponService>();
        controller = new WeaponController(mockService.Object);
    }

    [Fact]
    public async Task AddWeapon_ForFailedServiceResponse_ReturnsBadRequest()
    {
        // Arrange
        var weaponDto = new AddWeaponDto();
        var failedServiceResponse = new ServiceResponse<GetCharacterResponseDto>
        {
            Success = false,
            Message = "Invalid character id provided"
        };

        SetupMockService(weaponDto, failedServiceResponse);

        // Act
        var result = await controller.AddWeapon(weaponDto);

        //Assert
        TestHelper.CheckResponse(result, typeof(BadRequestObjectResult), failedServiceResponse);
    }

    [Fact]
    public async Task AddWeapon_WhenAddingAWeaponDTto_ReturnsGetCharacterResponseDto()
    {
        //Arrange 
        var weaponName = "Exkalibar";
        var characterId = 1;
        var addWeaponDto = new AddWeaponDto
        {
            CharacterId = characterId,
            Name = weaponName
        };
        var returningGetCharacterResponse = new GetCharacterResponseDto
        {
            Id = characterId,
            Weapon = new GetWeaponDto { Name = weaponName }
        };
        var serviceResponse = new ServiceResponse<GetCharacterResponseDto>
        {
            Data = returningGetCharacterResponse
        };

        SetupMockService(addWeaponDto, serviceResponse);

        //Act
        var result = await controller.AddWeapon(addWeaponDto);

        //Assert
        TestHelper.CheckResponse(result, typeof(OkObjectResult), serviceResponse);
    }

    private void SetupMockService(AddWeaponDto weaponDto,
        ServiceResponse<GetCharacterResponseDto> expectedServiceResponse)
    {
        mockService.Setup(service => service.AddWeaponToCharacter(weaponDto))
            .ReturnsAsync(expectedServiceResponse);
    }
}