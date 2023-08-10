using dotnet_rpg.Exceptions;
using dotnet_rpg.Repository;

namespace dotnet_rpg.Services.WeaponService;

public class WeaponService : IWeaponService
{
    private readonly IRepository<Character> _characterRepository;
    private readonly IRepository<Weapon> _weaponRepository;
    private readonly IMapper _mapper;

    public WeaponService(IRepository<Character> characterRepository, IRepository<Weapon> weaponRepository,
        IMapper mapper)
    {
        _characterRepository = characterRepository;
        _weaponRepository = weaponRepository;
        _mapper = mapper;
    }
    
    public async Task<ServiceResponse<GetCharacterResponseDto>> AddWeaponToCharacter(AddWeaponDto addWeaponDto)
    {
        var serviceResponse = new ServiceResponse<GetCharacterResponseDto>();
        try
        {
            var character = await _characterRepository.GetByIdAsync(addWeaponDto.CharacterId);
            if (character is null) throw new CharacterNotFoundException(addWeaponDto.CharacterId);
            var weapon = _mapper.Map<Weapon>(addWeaponDto);
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