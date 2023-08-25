namespace dotnet_rpg.Services.CharacterService;

public interface ICharacterService
{
    public Task<ServiceResponse<GetCharacterResponseDto>> GetCharacterById(int id);

    public Task<ServiceResponse<List<GetCharacterResponseDto>>> CreateCharacter(AddCharacterRequestDto newCharacter);

    public Task<ServiceResponse<List<GetCharacterResponseDto>>> GetAllCharacters();

    public Task<ServiceResponse<GetCharacterResponseDto>> UpdateCharacter(UpdateCharacterRequestDto updatedCharacter);
    
    public Task<ServiceResponse<List<GetCharacterResponseDto>>> DeleteCharacter(int id);

    public Task<ServiceResponse<GetCharacterResponseDto>> AddSkillToCharacter(
        AddCharacterSkillDto addCharacterSkillDto);
}