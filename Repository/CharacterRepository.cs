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

    public async Task<Character> GetByIdAsync(int characterId)
    {
        return (await _context.Characters
            .FirstOrDefaultAsync(c => c.Id == characterId && c.User!.Id.Equals(GetUserId())))!;
    }

    public Task<IEnumerable<Character>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public void Add(Character entity)
    {
        throw new NotImplementedException();
    }

    public void Delete(Character entity)
    {
        throw new NotImplementedException();
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