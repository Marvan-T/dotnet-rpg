using dotnet_rpg.Dtos.Fight;

namespace dotnet_rpg.Services.FightService;

public class FightService : IFightService
{
    private readonly IAttackPerformService _attackPerformService;
    private readonly ICharacterLookupService _characterLookupService;
    private readonly IRepository<Character> _characterRepository;
    private readonly IRandomGenerator _random;

    public FightService(IAttackPerformService attackPerformService, ICharacterLookupService characterLookupService,
        IRandomGenerator random, IRepository<Character> characterRepository)
    {
        _attackPerformService = attackPerformService;
        _characterLookupService = characterLookupService;
        _random = random;
        _characterRepository = characterRepository;
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
        var serviceResponse = new ServiceResponse<FightResultDto>
        {
            Data = new FightResultDto()
        };
        try
        {
            var characters = await _characterLookupService.FindCharactersByIds(fightRequestDto.CharacterIds);
            var defeated = false;

            while (!defeated)
                foreach (var attacker in characters)
                {
                    var opponents = characters.Where(c => c.Id != attacker.Id).ToList();
                    var opponent = opponents[_random.Next(opponents.Count)];

                    var attackType = _random.Next(2) == 0 ? "Skill" : "Weapon";
                    var damage = 0;

                    switch (attackType)
                    {
                        case "Skill" when attacker.Skills.Any():
                        {
                            var skill = attacker.Skills[_random.Next(attacker.Skills.Count)];
                            damage = DoSkillAttack(attacker, opponent, skill.Id);
                            break;
                        }
                        case "Weapon" when attacker.Weapon != null:
                            damage = DoWeaponAttack(attacker, opponent);
                            break;
                        default:
                            serviceResponse.Data.Log.Add($"{attacker.Name} decided to skip the turn.");
                            continue;
                    }

                    serviceResponse.Data.Log.Add(
                        $"{attacker.Name} deals {damage} to {opponent.Name} with a {attackType} attack.");

                    if (opponent.HitPoints <= 0)
                    {
                        defeated = true;
                        attacker.Victories++;
                        opponent.Defeats++;
                        serviceResponse.Data.Log.Add($"{opponent.Name} is defeated by {attacker.Name}");
                        serviceResponse.Data.Log.Add($"{attacker.Name} is victorious!");
                        break;
                    }
                }

            characters.ForEach(c =>
            {
                c.Fights++;
                c.HitPoints = 100;
            });

            await _characterRepository.SaveChangesAsync();
        }
        catch (Exception e)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = e.Message;
        }

        return serviceResponse;
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

    private int ApplyAttack(int baseDamage, int attackerModifier, int opponentDefense,
        Character opponent)
    {
        var damage = baseDamage + _random.Next(attackerModifier);
        damage -= _random.Next(opponentDefense);

        if (damage > 0) opponent.HitPoints -= damage;

        return damage;
    }
}