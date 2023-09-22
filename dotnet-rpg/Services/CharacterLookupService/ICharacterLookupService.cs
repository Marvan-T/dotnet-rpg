namespace dotnet_rpg.Services.CharacterLookupService;

public interface ICharacterLookupService
{
    public Task<Character> FindCharacterByUserAndCharacterId(int characterId);
    public Task<Character> FindCharacterByCharacterId(int characterId);
    public Task<List<Character>> FindCharactersByIds(List<int> characterIds);
}