using dotnet_rpg.Dtos.Skill;

namespace dotnet_rpg;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Character, GetCharacterResponseDto>();
        CreateMap<AddCharacterRequestDto, Character>();
        CreateMap<UpdateCharacterRequestDto, Character>();
        CreateMap<Weapon, GetWeaponDto>();
        CreateMap<AddWeaponDto, Weapon>();
        CreateMap<Skill, GetSkillDto>();
    }
}