using System.Security.Claims;

namespace dotnet_rpg.Repository;

public class CharacterRepository : IRepository<Character>
{
    private readonly DataContext _context;
    private readonly IHttpContextAccessor _iHttpContextAccessor;

    public CharacterRepository(DataContext dataContext, IHttpContextAccessor iHttpContextAccessor)
    {
        _context = dataContext;
        _iHttpContextAccessor = iHttpContextAccessor;
    }

    public Task<Character> GetByIdAsync(int characterId)
    {
        // First can be used but it returns an exception when the character is not found
        return _context.Characters
            .FirstOrDefaultAsync(c => c.Id == characterId && c.UserId.Equals(GetUserId()));
    }

    public Task<List<Character>> GetAllAsync()
    {
        return _context.Characters
            .Include(c => c.Weapon)
            .Where(c => c.UserId == GetUserId())
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
        throw new NotImplementedException();
    }

    public Task SaveChangesAsync()
    {
        throw new NotImplementedException();
    }

    private int GetUserId()
    {
        return int.Parse(_iHttpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}