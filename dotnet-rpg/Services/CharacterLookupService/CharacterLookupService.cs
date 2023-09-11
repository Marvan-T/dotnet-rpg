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
}