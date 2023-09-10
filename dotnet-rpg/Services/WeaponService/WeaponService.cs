using dotnet_rpg.Services.CharacterLookupService;

namespace dotnet_rpg.Services.WeaponService;

public class WeaponService : IWeaponService
{
    private readonly ICharacterLookupService _characterLookupService;
    private readonly IMapper _mapper;
    private readonly IRepository<Weapon> _weaponRepository;

    public WeaponService(ICharacterLookupService iCharacterLookupService, IRepository<Weapon> weaponRepository,
        IMapper mapper)
    {
        _characterLookupService = iCharacterLookupService;
        _weaponRepository = weaponRepository;
        _mapper = mapper;
    }

    public async Task<ServiceResponse<GetCharacterResponseDto>> AddWeaponToCharacter(AddWeaponDto addWeaponDto)
    {
        var serviceResponse = new ServiceResponse<GetCharacterResponseDto>();
        try
        {
            var character = await _characterLookupService.FindCharacterByUserAndCharacterId(addWeaponDto.CharacterId);
            var weapon = _mapper.Map<Weapon>(addWeaponDto);
            character.Weapon = weapon;
            _weaponRepository.Add(weapon);
            await _weaponRepository.SaveChangesAsync();
            serviceResponse.Data = _mapper.Map<GetCharacterResponseDto>(character);
        }
        catch (Exception e)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = e.Message;
        }

        return serviceResponse;
    }
}