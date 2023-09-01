# Entity Filters

Entity filters, or global query filters, are a feature in Entity Framework Core that allow you to apply a filter to
every query that gets executed for a certain entity type. The filter is a boolean lambda expression and you can use it
to sift out entities directly on the DbContext.
A common use case for query filters is in multi-tenant applications where each user has their own set of data in the
database. Another good example is to filter out soft deleted records so they will not be returned in normal queries.
Here's an example:

```csharp
public class YourDbContext : DbContext
{
    public DbSet<Skill> Skills { get; set; }

    public bool IsDeleted { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Skill>().HasQueryFilter(p => !p.IsDeleted);
    }
}
```

In this case, any query which is run against Skills in the database will always exclude any Skill where `IsDeleted`
property is `true`.
This way, you can ensure that deleted entities are never unintentionally included in your query results, but keep in
mind that they are still in the database, they're just being filtered out.