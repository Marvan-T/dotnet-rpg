using dotnet_rpg.Controllers;
using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Dtos.Weapon;
using dotnet_rpg.Models;
using dotnet_rpg.Services.WeaponService;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Tests.Controllers;

public class WeaponControllerTest
{
    [Fact]
    public async Task AddWeapon_ForFailedServiceResponse_ReturnsBadRequest()
    {
        // Arrange
        var weaponDto = new AddWeaponDto();
        var serviceResponseMessage = "Invalid character id provided";
        var failedServiceResponse = new ServiceResponse<GetCharacterResponseDto>
        {
            Success = false,
            Message = serviceResponseMessage
        };
        var mockService = new Mock<IWeaponService>();
        mockService.Setup(service => service.AddWeaponToCharacter(weaponDto))
            .ReturnsAsync(failedServiceResponse);
        var controller = new WeaponController(mockService.Object);

        // Act
        var result = await controller.AddWeapon(weaponDto);

        //Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result); // .Result returns ActionResultType
        var returnedServiceResponse =
            Assert.IsType<ServiceResponse<GetCharacterResponseDto>>(badRequestResult
                .Value); // .Value on what the bad request returns
        Assert.False(returnedServiceResponse.Success);
        Assert.Equal(returnedServiceResponse.Message, serviceResponseMessage);

        /* Note:
        in most cases when dealing with action results in ASP.NET Core, they come wrapped in a higher-order type like ActionResult<T>,
        so it's important to assert the type of the actual result that is included in this wrapper.
        */
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
        var getWeaponDto = new GetWeaponDto
        {
            Name = weaponName
        };
        var expectedGetCharacterResponseDto = new GetCharacterResponseDto
        {
            Id = characterId,
            Weapon = getWeaponDto
        };
        var serviceResponse = new ServiceResponse<GetCharacterResponseDto>
        {
            Data = expectedGetCharacterResponseDto
        };
        var mockService = new Mock<IWeaponService>();
        mockService.Setup(service => service.AddWeaponToCharacter(addWeaponDto))
            .ReturnsAsync(serviceResponse);
        var controller = new WeaponController(mockService.Object);

        //Act
        var result = await controller.AddWeapon(addWeaponDto);

        //Assert
        var okRequestResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedServiceResponse = Assert.IsType<ServiceResponse<GetCharacterResponseDto>>(okRequestResult
            .Value);
        Assert.Equal(returnedServiceResponse.Data, expectedGetCharacterResponseDto);
    }
}