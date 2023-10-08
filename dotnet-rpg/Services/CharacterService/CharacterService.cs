using dotnet_rpg.Services.Helper;

namespace dotnet_rpg.Services.CharacterService;

public class CharacterService : ICharacterService
{
    private readonly IAuthRepository _authRepository;
    private readonly ICharacterLookupService _characterLookupService;
    private readonly IRepository<Character> _characterRepository;
    private readonly IMapper _mapper;

    public CharacterService(IMapper mapper, IRepository<Character> characterRepository, IAuthRepository authRepository,
        ICharacterLookupService characterLookupService)
    {
        _mapper = mapper;
        _characterRepository = characterRepository;
        _authRepository = authRepository;
        _characterLookupService = characterLookupService;
    }

    public async Task<ServiceResponse<List<GetCharacterResponseDto>>> CreateCharacter(
        AddCharacterRequestDto newCharacter)
    {
        var serviceResponse = new ServiceResponse<List<GetCharacterResponseDto>>();
        var character = _mapper.Map<Character>(newCharacter);
        character.User = await _authRepository.GetByIdAsync(_authRepository.GetCurrentUserId());
        _characterRepository.Add(character);
        await _characterRepository.SaveChangesAsync();

        serviceResponse.Data = await FetchMappedCharacters();
        return serviceResponse;
    }

    public async Task<ServiceResponse<GetCharacterResponseDto>> GetCharacterById(int id)
    {
        var serviceResponse = new ServiceResponse<GetCharacterResponseDto>();
        try
        {
            var character = await _characterLookupService.FindCharacterByUserAndCharacterId(id);
            serviceResponse.Data = _mapper.Map<GetCharacterResponseDto>(character);
        }
        catch (Exception e)
        {
            ServiceResponseHelper.HandleServiceException(serviceResponse, e);
        }

        return serviceResponse;
    }

    public async Task<ServiceResponse<List<GetCharacterResponseDto>>> GetAllCharacters()
    {
        var serviceResponse = new ServiceResponse<List<GetCharacterResponseDto>>
        {
            Data = await FetchMappedCharacters()
        };
        return serviceResponse;
    }

    public async Task<ServiceResponse<GetCharacterResponseDto>> UpdateCharacter(
        UpdateCharacterRequestDto updatedCharacter)
    {
        var serviceResponse = new ServiceResponse<GetCharacterResponseDto>();

        try
        {
            var character = await _characterLookupService.FindCharacterByUserAndCharacterId(updatedCharacter.Id);
            _mapper.Map(updatedCharacter, character);

            // character.Name = updatedCharacter.Name;
            // character.HitPoints = updatedCharacter.HitPoints;
            // character.Strength = updatedCharacter.Strength;
            // character.Defense = updatedCharacter.Defense;
            // character.Class = updatedCharacter.Class;

            await _characterRepository.SaveChangesAsync();
            serviceResponse.Data = _mapper.Map<GetCharacterResponseDto>(character);
        }
        catch (Exception e)
        {
            ServiceResponseHelper.HandleServiceException(serviceResponse, e);
        }

        return serviceResponse;
    }

    public async Task<ServiceResponse<List<GetCharacterResponseDto>>> DeleteCharacter(int id)
    {
        var serviceResponse = new ServiceResponse<List<GetCharacterResponseDto>>();

        try
        {
            var character = await _characterLookupService.FindCharacterByUserAndCharacterId(id);
            _characterRepository.Delete(character);
            await _characterRepository.SaveChangesAsync();
            serviceResponse.Data = await FetchMappedCharacters();
        }
        catch (Exception e)
        {
            ServiceResponseHelper.HandleServiceException(serviceResponse, e);
        }

        return serviceResponse;
    }

    private async Task<List<GetCharacterResponseDto>> FetchMappedCharacters()
    {
        return _mapper.Map<List<GetCharacterResponseDto>>(await _characterRepository.GetAllAsync());
    }
}