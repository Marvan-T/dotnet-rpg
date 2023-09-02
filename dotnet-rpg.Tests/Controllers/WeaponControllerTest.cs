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
        var failedServiceResponse =
            TestHelper.CreateServiceResponse<GetCharacterResponseDto>(null, false, "Invalid character id provided");
        mockService.SetupMockServiceCall(service => service.AddWeaponToCharacter(weaponDto), failedServiceResponse);

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
        var serviceResponse = TestHelper.CreateServiceResponse(returningGetCharacterResponse);

        mockService.SetupMockServiceCall(service => service.AddWeaponToCharacter(addWeaponDto), serviceResponse);

        //Act
        var result = await controller.AddWeapon(addWeaponDto);

        //Assert
        TestHelper.CheckResponse(result, typeof(OkObjectResult), serviceResponse);
    }
}