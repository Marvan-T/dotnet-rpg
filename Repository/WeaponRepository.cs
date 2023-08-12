namespace dotnet_rpg.Repository;

public class WeaponRepository : IRepository<Weapon>
{
    private readonly DataContext _context;

    public WeaponRepository(DataContext dataContext)
    {
        _context = dataContext;
    }

    public Task<Weapon> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<List<Character>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public void Add(Weapon weapon)
    {
        _context.Add(weapon);
    }

    public void Delete(Weapon entity)
    {
        throw new NotImplementedException();
    }

    public void Update(Weapon entity)
    {
        throw new NotImplementedException();
    }

    public Task SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}