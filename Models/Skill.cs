namespace dotnet_rpg.Models;

public class Skill
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Damage { get; set; }
    public ICollection<Character> Characters { get; set; }
}