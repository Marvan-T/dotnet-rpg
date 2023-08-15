# Many-to-many

Consider the scenario where,

- A character could have many skills
- A skill could be holded by many characters

```csharp
public class Character
{
    public int Id { get; set; }
    public string CharacterName { get; set; }

    // A navigation property
    public ICollection<Skill> Skills { get; set; }
}
```

```csharp
public class Skill
{
    public int Id { get; set; }
    public string SkillName { get; set; }

    // A navigation property
    public ICollection<Character> Characters { get; set; }
}
```

The join table will be automatically created by EF core (since Entity Framework Core 5).

Make sure to add a DBSet to `DataContext`

```csharp
public class DataContext : DbContext
{
    public DbSet<Character> Characters => Set<Character>(); 
    ///...Other DbSets
    public DbSet<Skill> Skills => Set<Skill>();
}
```

- And lastly create and run the migrations
