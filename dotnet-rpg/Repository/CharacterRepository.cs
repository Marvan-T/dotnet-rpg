namespace dotnet_rpg.Repository;

public class CharacterRepository : BaseRepository<Character>
{
    private readonly IAuthRepository _authRepository;
    
    public CharacterRepository(DataContext dataContext, IAuthRepository authRepository)
        : base(dataContext)
    {
        _authRepository = authRepository;
    }

    public override Task<Character?> GetByIdAsync(int characterId)
    {
        // First can be used but it returns an exception when the character is not found
        return _dbContext.Characters
            .Include(c => c.Weapon)
            .Include(c => c.Skills)
            .FirstOrDefaultAsync(c => c.Id == characterId);
    }

    public override Task<List<Character>> GetAllAsync()
    {
        return _dbContext.Characters
            .Include(c => c.Weapon)
            .Include(c => c.Skills)
            .Where(c => c.UserId == _authRepository.GetCurrentUserId())
            .ToListAsync();
    }
    
    public override Task<List<Character>> GetByIdsAsync(List<int> characterIds)
    {
        return _dbContext.Characters
            .Include(c => c.Weapon)
            .Include(c => c.Skills)
            .Where(c => characterIds.Contains(c.Id))
            .ToListAsync();
    }
}