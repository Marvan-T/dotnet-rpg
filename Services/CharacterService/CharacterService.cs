using dotnet_rpg.Exceptions;
using dotnet_rpg.Repository;

namespace dotnet_rpg.Services.CharacterService;

public class CharacterService : ICharacterService
{
    private readonly IAuthRepository _authRepository;
    private readonly IRepository<Character> _characterRepository;
    private readonly IMapper _mapper;


    public CharacterService(IMapper mapper, IRepository<Character> characterRepository, IAuthRepository authRepository)
    {
        _mapper = mapper;
        _characterRepository = characterRepository;
        _authRepository = authRepository;
    }

    // This is how you make a method asynchronous async Task<ReturnType>, have to add await in the method call (see controller)
    public async Task<ServiceResponse<List<GetCharacterResponseDto>>> CreateCharacter(
        AddCharacterRequestDto newCharacter)
    {
        var serviceResponse = new ServiceResponse<List<GetCharacterResponseDto>>();
        var character = _mapper.Map<Character>(newCharacter);
        character.User = await _authRepository.GetByIdAsync(_authRepository.GetCurrentUserId());
        _characterRepository.Add(character);
        await _characterRepository.SaveChangesAsync();

        //approach 1    
        // _context.Characters.Add(character);
        // await _context.SaveChangesAsync();

        //approach 2 - , it's important to note that this AddAsync method doesn't make any database operations, it just provides an async way to add the entity to the DbContext
        /*
        There is not much benefit in using AddAsync over Add because the AddAsync method does not involve any I/O bound work,
        it just prepares the data to be saved and this is a fast operation.
        Async methods are mainly beneficial when there are I/O operations involved, like when calling SaveChangesAsync,
        which does interact with the database.
        */
        // await _context.Characters.AddAsync(character);
        // await _context.SaveChangesAsync();

        serviceResponse.Data = await FetchMappedCharacters();
        return serviceResponse;
    }

    public async Task<ServiceResponse<GetCharacterResponseDto>> GetCharacterById(int id)
    {
        var serviceResponse = new ServiceResponse<GetCharacterResponseDto>();
        var character = await _characterRepository.GetByIdAsync(id);
        serviceResponse.Data = _mapper.Map<GetCharacterResponseDto>(character);
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
            var character = await _characterRepository.GetByIdAsync(updatedCharacter.Id);

            if (character is null || character.UserId != _authRepository.GetCurrentUserId())
                throw new CharacterNotFoundException(updatedCharacter.Id);

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
            serviceResponse.Success = false;
            serviceResponse.Message = e.Message;
        }

        return serviceResponse;
    }

    public async Task<ServiceResponse<List<GetCharacterResponseDto>>> DeleteCharacter(int id)
    {
        var serviceResponse = new ServiceResponse<List<GetCharacterResponseDto>>();

        try
        {
            var character = await _characterRepository.GetByIdAsync(id) ?? throw new CharacterNotFoundException(id);
            _characterRepository.Delete(character);
            await _characterRepository.SaveChangesAsync();
            serviceResponse.Data = await FetchMappedCharacters();
        }
        catch (Exception e)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = e.Message;
        }

        return serviceResponse;
    }

    private async Task<List<GetCharacterResponseDto>> FetchMappedCharacters()
    {
        return _mapper.Map<List<GetCharacterResponseDto>>(await _characterRepository.GetAllAsync());
    }
}