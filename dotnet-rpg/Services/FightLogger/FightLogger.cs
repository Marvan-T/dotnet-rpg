using dotnet_rpg.Dtos.Fight;

namespace dotnet_rpg.Services.FightLogger;

public class FightLogger : IFightLogger
{
    public void LogSkipTurn(Character character, FightResultDto fightResult)
    {
        fightResult.Log.Add($"{character.Name} decided to skip the turn.");
    }

    public void LogVictory(Character attacker, Character opponent, FightResultDto fightResult)
    {
        fightResult.Log.Add($"{opponent.Name} is defeated by {attacker.Name}");
        fightResult.Log.Add($"{attacker.Name} is victorious!");
    }

    public void LogAttack(Character attacker, Character opponent, int damage, AttackType attackType,
        FightResultDto fightResult)
    {
        var logMessage = damage <= 0
            ? $"{attacker.Name} wasn't able to deal damage to {opponent.Name} with a {attackType} attack."
            : $"{attacker.Name} deals {damage} to {opponent.Name} with a {attackType} attack.";

        fightResult.Log.Add(logMessage);
    }
}