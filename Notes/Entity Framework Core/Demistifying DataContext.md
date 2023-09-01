# Todo: Needs a rewrite (some of the bits relating to the sue of Set<TEntity> seems to be incorrect)
# What is the DataContext
The DbContext class in Entity Framework (EF) represents a session with the database. It's a bridge between your .NET
application and your database. It's a part of Entity Framework – a technology that allows you to interact with your
database using .NET objects.
Here are some fundamental roles and uses of DbContext:

1. Querying: DbContext can execute Linq-to-Entities queries, compiled queries, or raw SQL queries, and return entities.
2. Change Tracking: It keeps track of changes that are made to the entities after they have been queried from the
   database.
3. Persisting Data: It carries out the Insert, Update, and Delete operations at the database level, based on the changes
   you made at entity level.
4. Caching: DbContext provides first-level cache by default. This means that repeated querying for the same data will
   not hit the database.
5. Manage Relationships: DbContext helps manage the relationships between entities and synchronizes changes (including
   updating the foreign keys).
6. Migrations: DbContext and the context of each model in your application are used to create database migrations, which
   manage and track changes to the database schema over time.

In general, you'd create one DbContext instance per request in a web application, or per scope in a desktop application.
Here is a simple example of a DbContext class:

Note:

- Name of the DbSet (property name) is the name of the corresponding database table.
- When you need a representation of a model in the database, you have to add a DbSet of the model.

```csharp
public class YourDbContext: DbContext
{
    public DbSet<Character> Characters { get; set; }
    public DbSet<Skill> Skills { get; set; }
}
```

The recommendation is to make these `DbSet` properties read only as having the setter would allow you to point the filed
to a different
table from elsewhere which does not make sense and would lead to confusing behaviour.

```csharp
public class YourDbContext: DbContext
{
    public DbSet<Character> Characters { get; }
    public DbSet<Skill> Skills { get; }
}
```

**Alternative implementation is as follows**:

This is using the ReadOnly auto properties where value is set inside the property itself. This is a semantically
read-only construct and it's perfectly fine and valid.

```csharp
public class YourDbContext: DbContext
{
    public DbSet<Character> Characters => Set<Character>():
    public DbSet<Skill> Skills => Set<Skill>():
}
```

In this example, we're using the DbContext's `Set<T>()` method to return a `DbSet<T>`. The DbContext's Set method "
Returns a DbSet instance for access to entities of the given type".
Using `Set<TEntity>()` is essentially equivalent to exposing a `DbSet<TEntity>` property per type (like
using `public DbSet<Character> Characters { get; }`) but it's somewhat more flexible (in specific scenarios - read
below).

## More on the Set<T>()
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

- The Set<Character>() method of the DbContext initializes Characters when it's first gotten or accessed. The Set<T>()
  method effectively says "Hey Entity Framework, give me the set that corresponds to the Character entities". After this
  initialization, the Characters property will continue to return this DbSet instance.
- The result of using Set<T>() is still a DbSet<Character>, and you can still change the data in the database that it
  represents. This happens because the DbSet<Character> itself provides methods to manipulate (add, update, remove) the
  Character entities in the database.

In other words:

- _This is saying when you call the `Characters` property it should return the result of calling the  `Set<Character>()`
  method which in turn returns a `DbSet<Character>`._

**The `Set<TEntity>()` or in this case `Set<Character>()` is coming from the base class `DbContext`**

## USage of Set<T> ()? Is bit more nuanced than i thought

The `Set<TEntity>()` method provides more flexibility in a multi-tenant application where each tenant may require
different custom entity types that are unknown until runtime.
A multi-tenant application is an application that serves multiple organizations or groups, called tenants, from a single
instance of the application. Each tenant might have their own set of entity types. For example, Tenant A might require
entities for "Orders," "Customers," and "Items," while Tenant B requires entities for "Courses," "Students," and "
Exams."
When you build an application for multiple tenants, you often won't know exactly what entities will be needed at the
time of compilation because they can vary from tenant to tenant. They might even change during runtime as tenants
customize their part of the application.
To account for these dynamic entity types, Entity Framework provides the `Set<TEntity>()` method. This method allows you
to work with entity types that you do not know about at compile time. Here is an example:

```csharp
public class YourDbContext : DbContext
{
    public DbSet<TEntity> Set<TEntity>(string tenant)
    {
    // Here, based on the tenant, you could decide what DbSet to return.
    }
}
```

Note well though: this is a very specific scenario and you will not come across this a lot. This just illustrate the
flexibility of the Set<T>() method.

In the example above, your code can call Set<YourEntity>() at runtime with the necessary type passed in as a parameter.
This can return different DbSets for different tenants.
On the other hand, if you define your DbSets like this:

```csharp
public DbSet<Character> Characters { get; }
public DbSet<Skill> Skills { get; }
```

Those entity types Character and Skill are known at compile time, and they can't be changed at runtime. You have decided
what types those DbSets will be, and it can't be easily customized for individual tenants who may need different
entities.