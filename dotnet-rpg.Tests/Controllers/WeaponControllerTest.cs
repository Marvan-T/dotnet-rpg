using System.Net;
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
        CheckResponse(result, typeof(BadRequestObjectResult), failedServiceResponse);
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
        CheckResponse(result,  typeof(OkObjectResult), serviceResponse);
    }
    
    private static void CheckResponse<T>(ActionResult<ServiceResponse<T>> result, Type expectedObjectResultType, ServiceResponse<T> expectedServiceResponse)
    {
        // Assert the type of the Result
        Assert.IsType(expectedObjectResultType, result.Result);

        // Then cast the result to its actual type to access its Value property
        var objectResult = (ObjectResult)result.Result;
        var response = Assert.IsType<ServiceResponse<T>>(objectResult.Value);

        // Assert that the Success properties are equal
        Assert.Equal(expectedServiceResponse.Success, response.Success);
    
        // Assert that the Message properties are equal
        Assert.Equal(expectedServiceResponse.Message, response.Message);
    
        // Assert that the Data properties are equal
        Assert.Equal(expectedServiceResponse.Data, response.Data);
    }
}