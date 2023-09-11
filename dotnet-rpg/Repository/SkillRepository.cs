namespace dotnet_rpg.Repository;

public class  SkillRepository : IRepository<Skill>
{
    private readonly DataContext _dataContext;

    public SkillRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public Task<Skill?> GetByIdAsync(int id)
    {
        return _dataContext.Skills.FirstOrDefaultAsync(c => c.Id == id);
    }

    public Task<List<Skill>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public void Add(Skill entity)
    {
        throw new NotImplementedException();
    }

    public void Delete(Skill entity)
    {
        throw new NotImplementedException();
    }

    public void Update(Skill entity)
    {
        throw new NotImplementedException();
    }

    public Task SaveChangesAsync()
    {
        return _dataContext.SaveChangesAsync();
    }
}