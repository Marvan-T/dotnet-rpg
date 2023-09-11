using dotnet_rpg.Services.CharacterLookupService;

namespace dotnet_rpg.Services.SkillService;

public class SkillService : ISkillService
{
    private readonly ICharacterLookupService _characterLookupService;
    private readonly IMapper _mapper;
    private readonly IRepository<Skill> _skillRepository;

    public SkillService(IRepository<Skill> skillRepository, ICharacterLookupService characterLookupService,
        IMapper mapper)
    {
        _skillRepository = skillRepository;
        _characterLookupService = characterLookupService;
        _mapper = mapper;
    }

    public async Task<ServiceResponse<GetCharacterResponseDto>> AddSkillToCharacter(
        AddCharacterSkillDto addCharacterSkillDto)
    {
        var serviceResponse = new ServiceResponse<GetCharacterResponseDto>();

        try
        {
            var character =
                await _characterLookupService.FindCharacterByUserAndCharacterId(addCharacterSkillDto.CharacterId);
            var skill = await FindSkill(addCharacterSkillDto.SkillId);
            AddCharacterSkill(character, addCharacterSkillDto.SkillId, skill);
            await _skillRepository.SaveChangesAsync();
            serviceResponse.Data = _mapper.Map<GetCharacterResponseDto>(character);
        }
        catch (Exception e)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = e.Message;
        }

        return serviceResponse;
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
}