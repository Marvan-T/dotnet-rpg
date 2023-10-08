namespace dotnet_rpg.Repository;

public class SkillRepository : BaseRepository<Skill>
{
    private readonly DataContext _dataContext;

    public SkillRepository(DataContext dataContext) : base(dataContext)
    {
        _dataContext = dataContext;
    }
}