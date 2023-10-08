namespace dotnet_rpg.Repository;

public class WeaponRepository : BaseRepository<Weapon>
{
    private readonly DataContext _dataContext;
    private readonly DataContext _context;

    public WeaponRepository(DataContext dataContext) : base(dataContext)
    {
        _dataContext = dataContext;
    }
    
}