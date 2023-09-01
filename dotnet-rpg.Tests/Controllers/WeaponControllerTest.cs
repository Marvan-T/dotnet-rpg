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
    private Mock<IWeaponService> mockService;
    private WeaponController controller;
    
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
        var returningGetCharacterResponse = new GetCharacterResponseDto
        {
            Id = characterId,
            Weapon = new GetWeaponDto{ Name = weaponName }
        };
        var serviceResponse = new ServiceResponse<GetCharacterResponseDto>
        {
            Data = returningGetCharacterResponse
        };

        SetupMockService(addWeaponDto, serviceResponse);

        //Act
        var result = await controller.AddWeapon(addWeaponDto);

        //Assert
        CheckResponse(result,  typeof(OkObjectResult), serviceResponse);
    }
    
    private void SetupMockService(AddWeaponDto weaponDto, ServiceResponse<GetCharacterResponseDto> expectedServiceResponse)
    {
        mockService.Setup(service => service.AddWeaponToCharacter(weaponDto))
            .ReturnsAsync(expectedServiceResponse);
    }
    
    private static void CheckResponse<T>(ActionResult<ServiceResponse<T>> result, Type expectedObjectResultType, ServiceResponse<T> expectedServiceResponse)
    {
        // Assert the type of the Result
        Assert.IsType(expectedObjectResultType, result.Result);

        // Then cast the result to its actual type to access its Value property
        var objectResult = (ObjectResult)result.Result;
        var response = Assert.IsType<ServiceResponse<T>>(objectResult.Value);
        
        //Assert against the properties of service response
        response.Success.Should().Be(expectedServiceResponse.Success);
        response.Message.Should().Be(expectedServiceResponse.Message);
        response.Data.Should().BeEquivalentTo(expectedServiceResponse.Data);
    }
}