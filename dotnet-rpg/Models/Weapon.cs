namespace dotnet_rpg.Models;

public class Weapon : IEntityWithId
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Damage { get; set; }
    public int CharacterId { get; set; }
    public Character Character { get; set; }
}