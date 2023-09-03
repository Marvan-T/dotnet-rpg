namespace dotnet_rpg.Exceptions;

public class SkillNotFoundException : Exception
{
    public SkillNotFoundException(int id) : base($"Skill with id: {id} not found.")
    {
    }
}