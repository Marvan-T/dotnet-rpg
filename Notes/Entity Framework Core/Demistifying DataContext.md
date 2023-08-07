
Have a look at this:

```csharp

{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Character> Characters => Set<Character>(); 
        
    }   
}
```
## What does the `=> Set<Character>();` mean in here?

This is equivalent to the expanded expression that is shown below

```csharp
public DbSet<Character> Characters 
{
    get 
    {
        return Set<Character>();
    }
}
```

This is saying when you call the `Characters` property it should return the result of calling the  `Set<Character>()` method which in turn returns a `DbSet<Character>`.

The `Set<TEntity>()` or in this case `Set<Character>()` is coming from the base class `DbContext`


