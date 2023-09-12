using dotnet_rpg.Dtos.Fight;
using dotnet_rpg.Services.CharacterLookupService;

namespace dotnet_rpg.Services.FightService;

public class FightService : IFightService
{
    private readonly ICharacterLookupService _characterLookupService;
    private readonly IRepository<Character> _characterRepository;

    public FightService(ICharacterLookupService characterLookupService, IRepository<Character> characterRepository)
    {
        _characterLookupService = characterLookupService;
        _characterRepository = characterRepository;
    }

    public async Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto weaponAttackDto)
    {
        var response = new ServiceResponse<AttackResultDto>();
        try
        {
            var attacker = await _characterLookupService.FindCharacterByUserAndCharacterId(weaponAttackDto.AttackerId);
            var opponent = await _characterLookupService.FindCharacterByCharacterId(weaponAttackDto.OpponentId);

            if (opponent.HitPoints <= 0)
            {
                response.Success = false;
                response.Message = $"{opponent.Name} has already been defeated";

                return response;
            }

            var damageDealt = DoWeaponAttack(attacker, opponent);
            UpdateFightStatistics(attacker, opponent);
            await _characterRepository.SaveChangesAsync();

            if (opponent.HitPoints <= 0) response.Message = $"{opponent.Name} has been defeated";

            response.Data = new AttackResultDto
            {
                Attacker = attacker.Name,
                Opponent = opponent.Name,
                AttackerHp = attacker.HitPoints,
                OpponentHp = opponent.HitPoints,
                DamageDealt = damageDealt
            };
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = e.Message;
        }

        return response;
    }

    private int DoWeaponAttack(Character attacker, Character opponent)
    {
        if (attacker.Weapon is null)
            throw new Exception("Attacker  has no weapon");

        var damage = attacker.Weapon.Damage + new Random().Next(attacker.Strength);
        damage -= new Random().Next(opponent.Defense);

        if (damage > 0) opponent.HitPoints -= damage;

        attacker.Fights += 1;
        opponent.Fights += 1;

        return damage;
    }

    private void UpdateFightStatistics(Character attacker, Character opponent)
    {
        if (opponent.HitPoints <= 0)
        {
            attacker.Victories += 1;
            opponent.Defeats += 1;
        }

        attacker.Fights += 1;
        opponent.Fights += 1;
    }
}