using dotnet_rpg.Dtos.Fight;

namespace dotnet_rpg.Services.FightService;

public class FightService : IFightService
{
    private readonly IAttackPerformService _attackPerformService;
    private readonly ICharacterLookupService _characterLookupService;
    private readonly IRepository<Character> _characterRepository;
    private readonly IFightLogger _fightLogger;
    private readonly IRandomGenerator _random;

    public FightService(IAttackPerformService attackPerformService, ICharacterLookupService characterLookupService,
        IRandomGenerator random, IRepository<Character> characterRepository, IFightLogger fightLogger)
    {
        _attackPerformService = attackPerformService;
        _characterLookupService = characterLookupService;
        _random = random;
        _characterRepository = characterRepository;
        _fightLogger = fightLogger;
    }

    public async Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto weaponAttackDto)
    {
        return await _attackPerformService.PerformAttack(weaponAttackDto,
            (attacker, opponent) => DoWeaponAttack(attacker, opponent));
    }

    public async Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackDto skillAttackDto)
    {
        return await _attackPerformService.PerformAttack(skillAttackDto,
            (attacker, opponent) => DoSkillAttack(attacker, opponent, skillAttackDto.SkillId));
    }

    public async Task<ServiceResponse<FightResultDto>> Fight(FightRequestDto fightRequestDto)
    {
        try
        {
            var characters = await _characterLookupService.FindCharactersByIds(fightRequestDto.CharacterIds);
            var fightResult = new FightResultDto();

            await ConductFightLoop(characters, fightResult);

            var MaxHitPoint = 100;
            characters.ForEach(c => { c.HitPoints = MaxHitPoint; });
            await _characterRepository.SaveChangesAsync();

            return new ServiceResponse<FightResultDto> { Data = fightResult };
        }
        catch (Exception e)
        {
            return new ServiceResponse<FightResultDto>
            {
                Success = false,
                Message = e.Message
            };
        }
    }

    private async Task ConductFightLoop(List<Character> characters, FightResultDto fightResult)
    {
        var defeated = false;

        while (!defeated)
            foreach (var attacker in characters)
            {
                defeated = await ProcessAttackForCharacter(attacker, characters, fightResult);
                if (defeated) break;
            }
    }

    private async Task<bool> ProcessAttackForCharacter(Character attacker, List<Character> characters,
        FightResultDto fightResult)
    {
        var opponent = GetRandomOpponent(attacker, characters);

        var (attackStrategy, attackType) = GetAttackStrategyAndType(attacker);

        var attackResult = await _attackPerformService.ExecuteAttack(attacker, opponent, attackStrategy);

        if (attackType == AttackType.Skip)
            _fightLogger.LogSkipTurn(attacker, fightResult);
        else
            _fightLogger.LogAttack(attacker, opponent, attackResult.DamageDealt, attackType, fightResult);

        if (opponent.HitPoints <= 0)
        {
            _fightLogger.LogVictory(attacker, opponent, fightResult);
            return true;
        }

        return false;
    }

    private (Func<Character, Character, int>, AttackType) GetAttackStrategyAndType(Character attacker)
    {
        var attackType = GetRandomAttackType(attacker);

        switch (attackType)
        {
            case AttackType.Skill:
                var skill = attacker.Skills[_random.Next(attacker.Skills.Count)];
                return ((a, o) => DoSkillAttack(a, o, skill.Id), AttackType.Skill);
            case AttackType.Weapon:
                return (DoWeaponAttack, AttackType.Weapon);
            default:
                return (SkipAttack, AttackType.Skip);
        }
    }


    private Character GetRandomOpponent(Character attacker, List<Character> characters)
    {
        var opponents = characters.Where(c => c.Id != attacker.Id).ToList();
        return opponents[_random.Next(opponents.Count)];
    }

    private AttackType GetRandomAttackType(Character character)
    {
        var attackOptions = new List<AttackType>();
        if (character.Weapon != null) attackOptions.Add(AttackType.Weapon);
        if (character.Skills.Any()) attackOptions.Add(AttackType.Skill);
        return attackOptions.Count == 0 ? AttackType.Skip : attackOptions[_random.Next(attackOptions.Count)];
    }

    private int DoWeaponAttack(Character attacker, Character opponent)
    {
        if (attacker.Weapon is null)
            throw new NoWeaponFoundException(attacker.Id);

        return ApplyAttack(attacker.Weapon.Damage, attacker.Strength, opponent.Defense, opponent);
    }

    private int DoSkillAttack(Character attacker, Character opponent, int skillId)
    {
        var skill = attacker.Skills.FirstOrDefault(s => s.Id == skillId);
        if (skill is null)
            throw new SkillNotFoundException(skillId);

        return ApplyAttack(skill.Damage, attacker.Intelligence, opponent.Defense, opponent);
    }

    private int SkipAttack(Character attacker, Character opponent)
    {
        return 0; // No damage is dealt in a skip action
    }

    private int ApplyAttack(int baseDamage, int attackerModifier, int opponentDefense,
        Character opponent)
    {
        var damage = baseDamage + _random.Next(attackerModifier);
        damage -= _random.Next(opponentDefense);

        if (damage > 0) opponent.HitPoints -= damage;

        return damage;
    }
}