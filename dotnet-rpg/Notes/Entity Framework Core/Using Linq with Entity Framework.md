# This is important

```csharp
var character = await _context.Characters.FirstOrDefaultAsync(c => c.Id == updatedCharacter.Id);
```

**This doesen't issue a `SELECT *`**

## Comparison with `findById`
Both `FirstOrDefaultAsync` in Entity Framework Core and `findById` in Spring Data JPA generate optimized SQL queries behind the scenes, and they will only fetch the required records based on the given id.

When you use `FirstOrDefaultAsync(c => c.Id == updatedCharacter.Id)`, Entity Framework Core doesn't issue a `SELECT * FROM Characters` query. Instead, it translates your LINQ query into a SQL query like `SELECT TOP(1) * FROM Characters WHERE Id = @id`. So it only fetches the first record that matches the given id, not the entire table.

Similarly, the `findById` method in Spring Data JPA will generate a SQL query like `SELECT * FROM Characters WHERE Id = ?`.

Both these queries are optimized to fetch only the required record, and the performance should be very similar, if not identical.

Remember that the benefits of using ORM tools like Entity Framework Core or Spring Data JPA include the abstraction of the underlying database operations and the ability to work with objects in your preferred programming language rather than writing SQL queries. They also take care of parameterization to avoid SQL injection attacks, among other things.


## LINQ and Entity Framework Core 
LINQ can indeed be used to filter collections in memory (like lists or arrays), which is somewhat similar to service layer filtering. However, **when used with Entity Framework (which is a LINQ provider), LINQ queries are not executed in memory, but are instead translated to SQL and executed in the database.**

This is a powerful feature because it allows you to write complex data queries using a consistent, type-safe language integrated with C#, and it ensures that your data filtering, sorting, and transformations are executed as efficiently as possible, regardless of the size of your dataset. In other words, you can work with your data at a high level of abstraction, and Entity Framework takes care of the details of translating your queries and executing them against the database.

