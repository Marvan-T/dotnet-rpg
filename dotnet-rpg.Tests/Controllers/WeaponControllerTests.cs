using dotnet_rpg.Controllers;
using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Dtos.Weapon;
using dotnet_rpg.Services.WeaponService;
using static dotnet_rpg.Tests.TestHelper;

namespace dotnet_rpg.Tests.Controllers;

public class WeaponControllerTests
{
    private readonly WeaponController controller;
    private readonly Mock<IWeaponService> mockService;

    public WeaponControllerTests()
    {
        mockService = new Mock<IWeaponService>();
        controller = new WeaponController(mockService.Object);
    }

    [Fact]
    public async Task AddWeapon_ForFailedServiceResponse_ReturnsBadRequest()
    {
        // Arrange
        var weaponDto = new AddWeaponDto();
        var expectedFailedServiceResponse =
            CreateServiceResponse<GetCharacterResponseDto>(null, false, "Invalid character id provided");
        mockService.SetupMockServiceCall(service => service.AddWeaponToCharacter(weaponDto),
            expectedFailedServiceResponse);

        // Act
        var result = await controller.AddWeapon(weaponDto);

        //Assert
        CheckResponse(result, typeof(BadRequestObjectResult), expectedFailedServiceResponse);
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
        var expectedServiceResponse = CreateServiceResponse(returningGetCharacterResponse);

        mockService.SetupMockServiceCall(service => service.AddWeaponToCharacter(addWeaponDto),
            expectedServiceResponse);

        //Act
        var result = await controller.AddWeapon(addWeaponDto);

        //Assert
        CheckResponse(result, typeof(OkObjectResult), expectedServiceResponse);
    }
}