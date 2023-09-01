namespace dotnet_rpg.Services.WeaponService;

public interface IWeaponService
{
    public Task<ServiceResponse<GetCharacterResponseDto>> AddWeaponToCharacter(AddWeaponDto addWeaponDto);
}