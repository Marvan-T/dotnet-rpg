# Automapper

AutoMapper profiles are classes that define how to map between different types in AutoMapper.

When you register AutoMapper in the dependency injection container, you can specify the assembly where the AutoMapper profiles are located. AutoMapper will then scan the assembly for any classes that inherit from the Profile class and automatically register them.

Each AutoMapper profile typically defines a set of mappings between two types. For example, you might have a profile that maps between a Character entity and a CharacterDto data transfer object:
```csharp
    public class CharacterProfile : Profile
    {
        public CharacterProfile()
        {
            CreateMap<Character, CharacterDto>();
            CreateMap<CharacterDto, Character>();
        }
    }
```
In this example, the CharacterProfile class defines two mappings: one from Character to CharacterDto, and one from CharacterDto to Character.

By registering the CharacterProfile class with AutoMapper, you can then use AutoMapper to automatically map between Character and CharacterDto instances:

```csharp
    var character = new Character { Name = "John Doe", Class = "Warrior" };
    var characterDto = _mapper.Map<CharacterDto>(character);
```

In this example, the _mapper instance is an instance of the IMapper interface, which is automatically registered with the dependency injection container when you register AutoMapper. The Map method is used to map between the Character and CharacterDto instances using the mappings defined in the CharacterProfile class.
