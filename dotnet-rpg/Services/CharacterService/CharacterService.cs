using dotnet_rpg.Exceptions;
using dotnet_rpg.Repository;

namespace dotnet_rpg.Services.CharacterService;

public class CharacterService : ICharacterService
{
    private readonly IAuthRepository _authRepository;
    private readonly IRepository<Character> _characterRepository;
    private readonly IMapper _mapper;
    private readonly IRepository<Skill> _skillRepository;

    public CharacterService(IMapper mapper, IRepository<Character> characterRepository, IAuthRepository authRepository,
        IRepository<Skill> skillRepository)
    {
        _mapper = mapper;
        _characterRepository = characterRepository;
        _authRepository = authRepository;
        _skillRepository = skillRepository;
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
            var character = await FindCharacterByUserAndCharacterId(id);
            serviceResponse.Data = _mapper.Map<GetCharacterResponseDto>(character);
        }
        catch (Exception e)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = e.Message;
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
            var character = await FindCharacterByUserAndCharacterId(updatedCharacter.Id);
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
            var character = await FindCharacterByUserAndCharacterId(id);
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

    public async Task<ServiceResponse<GetCharacterResponseDto>> AddSkillToCharacter(
        AddCharacterSkillDto addCharacterSkillDto)
    {
        var serviceResponse = new ServiceResponse<GetCharacterResponseDto>();

        try
        {
            var character = await FindCharacterByUserAndCharacterId(addCharacterSkillDto.CharacterId);
            var skill = await FindSkill(addCharacterSkillDto.SkillId);
            AddCharacterSkill(character, addCharacterSkillDto.SkillId, skill);
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

    private async Task<Character> FindCharacterByUserAndCharacterId(int characterId)
    {
        var character = await _characterRepository.GetByIdAsync(characterId);
        if (character == null || character.UserId != _authRepository.GetCurrentUserId()) 
            throw new CharacterNotFoundException(characterId);
        return character;
    }

    private async Task<Skill> FindSkill(int skillId)
    {
        var skill = await _skillRepository.GetByIdAsync(skillId);
        if (skill == null) throw new SkillNotFoundException(skillId);
        return skill;
    }

    private void AddCharacterSkill(Character character, int skillId, Skill skill)
    {
        if (character.Skills.Any(s => s.Id == skillId))
            throw new InvalidOperationException(
                $"Skill with ID {skillId} is already added to Character with ID {character.Id}");
        character.Skills.Add(skill);
    }

    private async Task<List<GetCharacterResponseDto>> FetchMappedCharacters()
    {
        return _mapper.Map<List<GetCharacterResponseDto>>(await _characterRepository.GetAllAsync());
    }
}