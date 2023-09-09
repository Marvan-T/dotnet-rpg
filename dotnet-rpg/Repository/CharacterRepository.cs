namespace dotnet_rpg.Repository;

public class CharacterRepository : IRepository<Character>
{
    private readonly IAuthRepository _authRepository;
    private readonly DataContext _context;

    public CharacterRepository(DataContext dataContext, IAuthRepository authRepository)
    {
        _context = dataContext;
        _authRepository = authRepository;
    }

    public Task<Character?> GetByIdAsync(int characterId)
    {
        // First can be used but it returns an exception when the character is not found
        return _context.Characters
            .Include(c => c.Weapon)
            .Include(c => c.Skills)
            .FirstOrDefaultAsync(c => c.Id == characterId);
    }

    public Task<List<Character>> GetAllAsync()
    {
        return _context.Characters
            .Include(c => c.Weapon)
            .Include(c => c.Skills)
            .Where(c => c.UserId == _authRepository.GetCurrentUserId())
            .ToListAsync();
    }

    public void Add(Character character)
    {
        _context.Characters.Add(character);
    }

    public void Delete(Character character)
    {
        _context.Remove(character);
    }

    public void Update(Character entity)
    {
        //Todo: update doesn't seems to be necessary but lookup!
        throw new NotImplementedException();
    }

    public Task SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}