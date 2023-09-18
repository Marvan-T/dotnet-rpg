using dotnet_rpg.Controllers;
using dotnet_rpg.Dtos.Fight;
using dotnet_rpg.Services.FightService;
using static dotnet_rpg.Tests.TestHelper;

namespace dotnet_rpg.Tests.Controllers;

public class FightControllerTests
{
    private readonly FightController fightController;
    private readonly Mock<IFightService> mockFightService;

    public FightControllerTests()
    {
        mockFightService = new Mock<IFightService>();
        fightController = new FightController(mockFightService.Object);
    }

    [Fact]
    public async Task DoWeaponAttack_WhenAttackIsSuccessful_ReturnsAttackResultDto()
    {
        // Arrange
        var weaponAttackDto = new WeaponAttackDto();
        var attackResultDto = new AttackResultDto();
        var expectedServiceResponse = CreateServiceResponse(attackResultDto);
        mockFightService.SetupMockServiceCall(service => service.WeaponAttack(weaponAttackDto),
            expectedServiceResponse);

        // Act
        var result = await fightController.DoWeaponAttack(weaponAttackDto);

        // Assert
        CheckResponse(result, typeof(OkObjectResult), expectedServiceResponse);
    }

    [Fact]
    public async Task DoWeaponAttack_WhenAttackFails_ReturnsBadRequest()
    {
        // Arrange
        var weaponAttackDto = new WeaponAttackDto();
        var expectedFailedServiceResponse =
            CreateServiceResponse<AttackResultDto>(null, false, "Invalid attack parameters");
        mockFightService.SetupMockServiceCall(service => service.WeaponAttack(weaponAttackDto),
            expectedFailedServiceResponse);

        // Act
        var result = await fightController.DoWeaponAttack(weaponAttackDto);

        // Assert
        CheckResponse(result, typeof(BadRequestObjectResult), expectedFailedServiceResponse);
    }

    [Fact]
    public async Task DoSkillAttack_WhenAttackIsSuccessful_ReturnsOkWithServiceResponse()
    {
        // Arrange 
        var skillAttackDto = new SkillAttackDto();
        var attackResultDto = new AttackResultDto();
        var expectedServiceResponse = CreateServiceResponse(attackResultDto);

        mockFightService.Setup(x => x.SkillAttack(skillAttackDto)).ReturnsAsync(expectedServiceResponse);

        // Act
        var result = await fightController.DoSkillAttack(skillAttackDto);

        // Assert
        CheckResponse(result, typeof(OkObjectResult), expectedServiceResponse);
    }

    [Fact]
    public async Task DoSkillAttack_WhenAttackFails_ReturnsBadRequestWithServiceResponse()
    {
        // Arrange 
        var skillAttackDto = new SkillAttackDto();

        var expectedFailedServiceResponse =
            CreateServiceResponse<AttackResultDto>(null, false, "Attack failed due to some reason");
        mockFightService.Setup(x => x.SkillAttack(skillAttackDto)).ReturnsAsync(expectedFailedServiceResponse);

        // Act
        var result = await fightController.DoSkillAttack(skillAttackDto);

        // Assert
        CheckResponse(result, typeof(BadRequestObjectResult), expectedFailedServiceResponse);
    }
}