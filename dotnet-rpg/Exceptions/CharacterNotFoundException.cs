namespace dotnet_rpg.Exceptions;

public class CharacterNotFoundException : Exception
{
    public CharacterNotFoundException(int id) : base($"Character with id: {id} not found.")
    {
    }

    public CharacterNotFoundException(List<int> ids)
        : base($"Characters with the following IDs were not found: {string.Join(", ", ids)}")
    {
    }
}