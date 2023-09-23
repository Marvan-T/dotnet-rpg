using dotnet_rpg.Dtos.Fight;

namespace dotnet_rpg.Services.AttackPerformService;

public class AttackPerformService : IAttackPerformService
{
    private readonly ICharacterLookupService _characterLookupService;
    private readonly IRepository<Character> _characterRepository;

    public AttackPerformService(ICharacterLookupService characterLookupService,
        IRepository<Character> characterRepository)
    {
        _characterLookupService = characterLookupService;
        _characterRepository = characterRepository;
    }

    public async Task<ServiceResponse<AttackResultDto>> PerformAttack(IAttackDto attackDto,
        Func<Character, Character, int> attackStrategy)
    {
        var response = new ServiceResponse<AttackResultDto>();
        try
        {
            var attacker = await _characterLookupService.FindCharacterByUserAndCharacterId(attackDto.AttackerId);
            var opponent = await _characterLookupService.FindCharacterByCharacterId(attackDto.OpponentId);

            if (IsDefeated(opponent)) return BuildDefeatedResponse(opponent, response);

            var damageDealt = attackStrategy.Invoke(attacker, opponent);
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

    public async Task<AttackResultDto> ExecuteAttack(Character attacker, Character opponent,
        Func<Character, Character, int> attackStrategy)
    {
        if (IsDefeated(opponent)) throw new Exception($"{opponent.Name} has already been defeated");

        var damageDealt = attackStrategy.Invoke(attacker, opponent);
        UpdateFightStatistics(attacker, opponent);

        return BuildAttackResult(attacker, opponent, damageDealt);
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

    private AttackResultDto BuildAttackResult(Character attacker, Character opponent, int damageDealt)
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