namespace dotnet_rpg.Models;

public class Character
{
    public int Id { get; set; }
    public string Name { get; set; } = "Jerry"; //specifying defaults (you can use ? to make it nullable)
    public int HitPoints { get; set; } = 100;
    public int Strength { get; set; } = 10;
    public int Defense { get; set; } = 10;
    public int Intelligence { get; set; }
    public RpgClass Class { get; set; } = RpgClass.Knight;
    public int UserId { get; set; } // not really required, see the notes on One-to-many relationship

    /**
     * Why this is nullable and UserId is not nullable?
     * -------------------------------------------------
     * User is a navigation property. Its purpose is to allow you to navigate from a Character entity to a related User entity in your C# code.
     * It's okay for User to be null because there are valid situations where you might have a Character instance without its related User loaded.
     * For instance, you might want to retrieve a list of characters without loading the user data to save bandwidth and improve performance. In this case, User will be null.
     * 
     * UserId is a foreign key property. It represents the relationship between Character and User in the database.
     * In relational databases, the concept of "nullability" for foreign key columns is different from the concept of nullability for object references in C#.
     * If a foreign key column in a database row has a value, it means that row is related to another row. If the foreign key column is null, it means the row is not related to any row in the other table.
     * 
     * In this case, every Character is related to a User - that is, every character must have a user. That's why UserId is non-nullable. If UserId were nullable,
     * it would mean that you could have characters without users, which is not the case according to the domain model.
     * *
     */
    public User? User { get; set; }

    public Weapon? Weapon { get; set; }

    public ICollection<Skill> Skills { get; set; }
    public int Fights { get; set; }
    public int Victories { get; set; }
    public int Defeats { get; set; }
}