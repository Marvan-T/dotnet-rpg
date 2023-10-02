namespace dotnet_rpg.Services.CharacterLookupService;

public class CharacterLookupService : ICharacterLookupService
{
    private readonly IAuthRepository _authRepository;
    private readonly IRepository<Character> _characterRepository;

    public CharacterLookupService(IRepository<Character> characterRepository, IAuthRepository authRepository)
    {
        _characterRepository = characterRepository;
        _authRepository = authRepository;
    }

    public async Task<Character> FindCharacterByUserAndCharacterId(int characterId)
    {
        var character = await _characterRepository.GetByIdAsync(characterId);
        if (character == null || character.UserId != _authRepository.GetCurrentUserId())
            throw new CharacterNotFoundException(characterId);

        return character;
    }

    public async Task<Character> FindCharacterByCharacterId(int characterId)
    {
        var character = await _characterRepository.GetByIdAsync(characterId);
        if (character == null)
            throw new CharacterNotFoundException(characterId);
        return character;
    }

    public async Task<List<Character>> FindCharactersByIds(List<int> characterIds)
    {
        var characters = await _characterRepository.GetByIdsAsync(characterIds);

        var retrievedCharacterIds = characters.Select(c => c.Id).ToList();
        var missingCharacterIds = characterIds.Except(retrievedCharacterIds).ToList();

        if (missingCharacterIds.Any())
            throw new CharacterNotFoundException(missingCharacterIds);

        return characters;
    }
}