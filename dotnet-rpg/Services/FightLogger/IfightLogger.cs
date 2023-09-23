using dotnet_rpg.Dtos.Fight;

namespace dotnet_rpg.Services.FightLogger;

public interface IFightLogger
{
    void LogSkipTurn(Character character, FightResultDto fightResult);

    void LogAttack(Character attacker, Character opponent, int damage, AttackType attackType,
        FightResultDto fightResult);

    void LogVictory(Character attacker, Character opponent, FightResultDto fightResult);
}