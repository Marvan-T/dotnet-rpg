namespace dotnet_rpg.Data;

/*
  You can use an instance of DbContext to interact with the database (query)
*/
public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
            
    }
    
    public DbSet<Character> Characters => Set<Character>(); 
    public DbSet<User> Users => Set<User>();
    public DbSet<Weapon> Weapons => Set<Weapon>();

}