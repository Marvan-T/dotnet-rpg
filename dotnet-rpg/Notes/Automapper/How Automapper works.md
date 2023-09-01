# Brief overview on how Automapper works

Consider the following DTO's:

```csharp
public class GetCharacterResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "Jerry"; 
    public int HitPoints { get; set; } = 100;
    public int Strength { get; set; } = 10;
    public int Defense { get; set; } = 10;
    public RpgClass Class { get; set; } = RpgClass.Knight;
    public GetWeaponDto? Weapon { get; set; }
    public ICollection<GetSkillDto>? Skills { get; set; } // this property is what the discssion will be focussing on
}

public class GetSkillDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Damage { get; set; }
}
```

Our `Character` and `Skill` model looks like this

```csharp
public class Character
{
    public int Id { get; set; }
    public string Name { get; set; } = "Jerry"; 
    public int HitPoints { get; set; } = 100;
    public int Strength { get; set; } = 10;
    public int Defense { get; set; }  = 10;
    public RpgClass Class { get; set; }  = RpgClass.Knight;
    public int UserId { get; set; } 
    public User? User { get; set; }
    public Weapon? Weapon { get; set; }
    public ICollection<Skill> Skills { get; set; }

}

public class Skill
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Damage { get; set; }
    public ICollection<Character> Characters { get; set; }
}
```

To automatically map `Character` to `GetCharacterResponseDto` from Automapper
(i.e. `_mapper.Map<GetCharacterResponseDto>(character);`) we need to have the following Automapper configuration
defined.

```csharp
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
```

## How it works explained through the Skills property in the `Character`

The mapping from Skill to GetSkillDto indeed doesn't directly tell AutoMapper how to associate a GetSkillDto with a
GetCharacterResponseDto.
The connection is made through the Character to GetCharacterResponseDto mapping.
When AutoMapper sees that it needs to map a Character to a GetCharacterResponseDto, it will look at each property in the
Character class and attempt to map it to a corresponding property in the GetCharacterResponseDto class.
When it comes to the Skills property in Character, which is a collection of Skill objects, AutoMapper will attempt to
map each Skill in that collection to a GetSkillDto — because you've stated in your mapping configuration that a Skill
corresponds to a GetSkillDto. It then populates the Skills property in the GetCharacterResponseDto object with this
newly created collection of GetSkillDto objects.

Here's a rough step-by-step of what happens:

1. AutoMapper begins mapping a Character to a GetCharacterResponseDto.
2. When it encounters a property in Character that's a complex type or a collection (like Skills), it checks if there's
   a mapping defined for that type.
3. Seeing that there's a mapping from Skill to GetSkillDto, AutoMapper uses that mapping to convert each Skill in Skills
   to a GetSkillDto.
4. These mapped GetSkillDto objects are used to populate the Skills property in the GetCharacterResponseDto.

By setting up these mappings, you're telling AutoMapper how to convert between these types at the property level, and
AutoMapper takes care of applying these conversions to collections of these types and nested properties.