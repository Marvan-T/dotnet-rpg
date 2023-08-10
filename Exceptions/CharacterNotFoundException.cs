namespace dotnet_rpg.Exceptions;

public class CharacterNotFoundException : Exception
{
    public CharacterNotFoundException(int id) : base($"Character with id: {id} not found")
    {
    }
}