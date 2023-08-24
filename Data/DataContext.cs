namespace dotnet_rpg.Data;

/*
  You can use an instance of DbContext to interact with the database (query)
*/
public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
            
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Skill>().HasData(
            new Skill { Id = 1, Name = "Fireball", Damage = 5 },
            new Skill { Id = 2, Name = "Thunderbolt", Damage = 8 },
            new Skill { Id = 3, Name = "Ice Shard", Damage = 7 }
        );
    }

    public DbSet<Character> Characters => Set<Character>(); 
    public DbSet<User> Users => Set<User>();
    public DbSet<Weapon> Weapons => Set<Weapon>();
    public DbSet<Skill> Skills { get; set; } // the other approach for defining the DBSet property
}