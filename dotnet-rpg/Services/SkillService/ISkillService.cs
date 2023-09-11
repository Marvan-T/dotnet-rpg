namespace dotnet_rpg.Services.SkillService;

public interface ISkillService
{
    public Task<ServiceResponse<GetCharacterResponseDto>> AddSkillToCharacter(
        AddCharacterSkillDto addCharacterSkillDto);
}