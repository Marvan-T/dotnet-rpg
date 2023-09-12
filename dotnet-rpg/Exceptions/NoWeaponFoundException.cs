namespace dotnet_rpg.Exceptions;

public class NoWeaponFoundException : Exception
{
    public NoWeaponFoundException(int id) : base($"Attacker with ID {id} does not have a weapon.")
    {
    }
}