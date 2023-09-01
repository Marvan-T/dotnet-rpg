# IQueryable and DbSet

See this:

```csharp
public Task<List<Character>> GetAllAsync()
{
    return _context.Characters
        .Include(c => c.Weapon)
        .Where(c => c.UserId == GetUserId())
        .ToListAsync();
}
```

- You're using ToListAsync() in your GetAllAsync method, which makes LINQ execute the query immediately and retrieve all
  characters from the database that match your filter conditions. ToListAsync is an Entity Framework extension method
  that enumerates the results of the query and returns a List.
- You would consider using IQueryable or DbSet when you want to build up a query over multiple steps or want to defer
  execution of the query to a later point. They represent a query that can be enumerated, but hasn't been enumerated
  yet. In the above example, we've already built up the entire query and want to execute it immediately, so ToListAsync
  is the right choice.

**Note about using `async` in the above method**

- You don't need `async` `await` in here if you don't plan to use the result of this query. So returning a `Task` is
  sufficient. Then when you call this method from the service layer use `await`.

## Note about exceptions when returning tasks

This is the same code as above:

```csharp
  public Task<List<Character>> GetAllAsync()
    {
        return _context.Characters
            .Include(c => c.Weapon)
            .Where(c => c.UserId == GetUserId())
            .ToListAsync();
    }
```

- When you call GetAllAsync() without await, it returns a Task<List<Character>>. If an exception occurs during the
  execution of the task, that exception is stored in the returned task and it's thrown only when you await the task or
  access the Result or Exception properties of the task.
- Conversely, if you do use await when calling GetAllAsync(), like so: await GetAllAsync(), then if an exception occurs
  within GetAllAsync(), that exception will be thrown immediately at the call site.
- The important thing is that even when using async/await, exceptions are delivered to you. They just might be delivered
  at a different place in your code depending on when and how you consume the Task.