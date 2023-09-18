using dotnet_rpg.Dtos.Skill;

namespace dotnet_rpg.Dtos.Character;

public class GetCharacterResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "Jerry"; //specifying defaults (you can use ? to make it nullable)
    public int HitPoints { get; set; } = 100;
    public int Strength { get; set; } = 10;
    public int Defense { get; set; } = 10;
    public int Intelligence { get; set; }
    public RpgClass Class { get; set; } = RpgClass.Knight;
    public GetWeaponDto? Weapon { get; set; }
    public ICollection<GetSkillDto>? Skills { get; set; }
    public int Fights { get; set; }
    public int Victories { get; set; }
    public int Defeats { get; set; }
}