using dotnet_rpg.Dtos.Fight;
using dotnet_rpg.Services.CharacterLookupService;
using dotnet_rpg.Utility.RandomGeneration;

namespace dotnet_rpg.Services.FightService;

public class FightService : IFightService
{
    private readonly ICharacterLookupService _characterLookupService;
    private readonly IRepository<Character> _characterRepository;
    private readonly IRandomGenerator _random;

    public FightService(ICharacterLookupService characterLookupService, IRepository<Character> characterRepository,
        IRandomGenerator random)
    {
        _characterLookupService = characterLookupService;
        _characterRepository = characterRepository;
        _random = random;
    }

    public async Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto weaponAttackDto)
    {
        var response = new ServiceResponse<AttackResultDto>();
        try
        {
            var attacker = await _characterLookupService.FindCharacterByUserAndCharacterId(weaponAttackDto.AttackerId);
            var opponent = await _characterLookupService.FindCharacterByCharacterId(weaponAttackDto.OpponentId);

            if (IsDefeated(opponent)) return BuildDefeatedResponse(opponent, response);

            var damageDealt = DoWeaponAttack(attacker, opponent);
            UpdateFightStatistics(attacker, opponent);
            await _characterRepository.SaveChangesAsync();

            if (IsDefeated(opponent)) response.Message = $"{opponent.Name} has been defeated";

            response.Data = BuildAttackResultDto(attacker, opponent, damageDealt);
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = e.Message;
        }

        return response;
    }

    private bool IsDefeated(Character character)
    {
        return character.HitPoints <= 0;
    }

    private ServiceResponse<AttackResultDto> BuildDefeatedResponse(Character opponent,
        ServiceResponse<AttackResultDto> response)
    {
        response.Success = false;
        response.Message = $"{opponent.Name} has already been defeated";
        return response;
    }

    private int DoWeaponAttack(Character attacker, Character opponent)
    {
        if (attacker.Weapon is null)
            throw new NoWeaponFoundException(attacker.Id);

        var damage = attacker.Weapon.Damage + _random.Next(attacker.Strength);
        damage -= _random.Next(opponent.Defense);

        if (damage > 0) opponent.HitPoints -= damage;

        return damage;
    }

    private void UpdateFightStatistics(Character attacker, Character opponent)
    {
        attacker.Fights += 1;
        opponent.Fights += 1;

        if (IsDefeated(opponent))
        {
            attacker.Victories += 1;
            opponent.Defeats += 1;
        }
    }

    private AttackResultDto BuildAttackResultDto(Character attacker, Character opponent, int damageDealt)
    {
        return new AttackResultDto
        {
            Attacker = attacker.Name,
            Opponent = opponent.Name,
            AttackerHp = attacker.HitPoints,
            OpponentHp = opponent.HitPoints,
            DamageDealt = damageDealt
        };
    }
}