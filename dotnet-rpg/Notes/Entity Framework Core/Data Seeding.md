# Data Seeding (aka inserting data)

Sure, you can seed data in Entity Framework Core by using the HasData method inside your DbContext.
To do this, you should first create a new DbContext class if you haven't done so. The DbContext is essentially a session
in your database, enabling you to query and save instances of your entities.

1. The first thing you need to do is override the OnModelCreating method in your context:

```csharp
public class YourDbContext : DbContext
{
public DbSet<Skill> Skills { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Skill>().HasData(
            new Skill{ Id = 1, Name = "Fireball", Damage = 5 },
            new Skill{ Id = 2, Name = "Thunderbolt", Damage = 8 },
            new Skill{ Id = 3, Name = "Ice Shard", Damage = 7 }
        );
        // if you are defining relationships you need to provide the related foreign key Id's in here
    }
}
```

In this example, three skills are seeded into the database.

2. You then need to run a migration for Entity Framework to pick up this change.

```shell
dotnet ef migrations add SeedSkills
```

3. And then update your database.

```shell
dotnet ef database update
```

## About `ModelBuilder`

`ModelBuilder` is class provided by Entity Framework Core. It is typically used within the `OnModelCreating` method in
your `DbContext` class to **further configure the model that was discovered by convention, specify entity types to be
included in the model, and configure aspects of the model that cannot be discovered by convention**.
In terms of data seeding, ModelBuilder has an extension method called HasData which you can use to specify the seed data
for a specific entity.
The ModelBuilder is also used to define relationships between entities, configure how entities are mapped to a specific
table, and specify which columns are used as primary or foreign keys.
Here is an example of how it can be used:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Configure entity filters
    modelBuilder.Entity<Skill>().HasQueryFilter(p => !p.IsDeleted);

    // Configure relationships
    modelBuilder.Entity<Order>()
        .HasOne(o => o.Customer)
        .WithMany(c => c.Orders)
        .HasForeignKey(o => o.CustomerId);

    // Configure value conversions
    modelBuilder.Entity<Order>()
        .Property(o => o.Status)
        .HasConversion(
            v => v.ToString(),
            v => (OrderStatus)Enum.Parse(typeof(OrderStatus), v));

    // Seed data
    modelBuilder.Entity<Skill>().HasData(
        new Skill { Id = 1, Name = "Fireball", Damage = 5 },
        new Skill { Id = 2, Name = "Thunderbolt", Damage = 8 }
    );
}
```

The `ModelBuilder` essentially describes the shape of your entities, their relationships, and how they map to the
database.
