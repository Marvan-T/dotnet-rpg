using System.Security.Claims;

namespace dotnet_rpg.Services.WeaponService;

public class WeaponService : IWeaponService
{
    private readonly DataContext _context;
    private readonly IHttpContextAccessor _iHttpContextAccessor;
    private readonly IMapper _mapper;

    public WeaponService(DataContext context, IHttpContextAccessor iHttpContextAccessor, IMapper mapper)
    {
        _context = context;
        _iHttpContextAccessor = iHttpContextAccessor;
        _mapper = mapper;
    }
    
    public async Task<ServiceResponse<GetCharacterResponseDto>> AddWeaponToCharacter(AddWeaponDto addWeaponDto)
    {
        var serviceResponse = new ServiceResponse<GetCharacterResponseDto>();
        try
        {
            var validatedCharacter = await ValidateAndGetCharacter(addWeaponDto.CharacterId);
            var weapon = _mapper.Map<Weapon>(addWeaponDto);
            _context.Weapons.Add(weapon);
            await _context.SaveChangesAsync();
            serviceResponse.Data = _mapper.Map<GetCharacterResponseDto>(validatedCharacter);
        }
        catch (Exception e)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = e.Message;
        }
        return serviceResponse;
    }

    private async Task<Character> ValidateAndGetCharacter(int characterId)
    {
        var character = await _context.Characters
            .FirstOrDefaultAsync(c => c.Id == characterId && c.User!.Id.Equals(GetUserId()));
        if (character == null)
        {
            throw new Exception($"Character with id: {characterId} not found");
        }
        return character;
    }

    private int GetUserId() => int.Parse(_iHttpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    
}