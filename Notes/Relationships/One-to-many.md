# One-to-many relationship

Let's illustrate this with an example where one `Author` can have many `Books`.

First, you would define your `Author` and `Book` entities:

```csharp
public class Author
{
    public int Id { get; set; }
    public string Name { get; set; }

    // Navigation property
    public ICollection<Book> Books { get; set; }
}

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int AuthorId { get; set; }  // Foreign Key 

    // Navigation property
    public Author Author { get; set; }
}
```
In this code, the `Author` entity has a collection of `Book` entities (`ICollection<Book> Books`), and the `Book` entity has a single `Author` (`public Author Author`). This is what establishes the one-to-many relationship: one author can have many books, but each book has one and only one author.

The `AuthorId` in the `Book` class is a foreign key. It specifies which author a book belongs to.

Then, in your `DbContext` subclass, you would have:

```csharp
public class DataContext : DbContext
{
    public DbSet<Author> Authors { get; set; }
    public DbSet<Book> Books { get; set; }
}
```

Entity Framework will automatically infer the one-to-many relationship from the navigation properties in your entities. When you generate a migration and update your database, it will create the `Authors` and `Books` tables with the appropriate foreign key relationship.

In some cases, if you need to configure the relationship in more detail (like adding constraints), you can override the `OnModelCreating` method in your `DbContext` subclass. But for simple one-to-many relationships like this, Entity Framework can handle it automatically.

## It is not strictly necessary to include the `AuthorId`

"In some cases, you may not want a foreign key property in your model, since foreign keys are a detail of how the relationship is represented in the database, which is not needed when using the relationship in a purely object-oriented manner. However, **if entities are going to be serialized, for example to send over a wire, then the foreign key values can be a useful way to keep the relationship information intact when the entities are not in an object form**. It is therefore often pragmatic to keep foreign key properties in the .NET type for this purpose. Foreign key properties can be private, which is often a good compromise to avoid exposing the foreign key while allowing its value to travel with the entity.""

This statement is referring to the utility of including a foreign key property in the model from a couple of different perspectives.

1. **Direct access to the foreign key**: When you have a foreign key property in your model, you can directly access the key's value without needing to load the related entity. This can be beneficial for performance, as it avoids the need for potentially expensive database operations to load related entities.

2. **Serialization**: If you serialize an entity and send it somewhere else (i.e., "over a wire"), the foreign key value can maintain the relationship information without needing to include the entire related entity. This can reduce the size of the serialized data and help maintain the integrity of your data relationships when the data is deserialized. For example, if you were to serialize a `Book` object and send it to a client application, the client could still understand what `Author` the `Book` is related to by looking at the `AuthorId` property, even if the `Author` object itself isn't included in the serialized data.

3. **Object-relational impedance mismatch**: This is a more complex issue that arises from the fact that object-oriented programming and relational databases are based on different paradigms, and there can be some friction when trying to map between the two. **Including the foreign key in your object model can help smooth over some of these issues by making the mapping between the object model and the database schema more explicit.**

<blockquote class="callout">
To be clear, <b>including the foreign key property in your model isn't strictly necessary. Entity Framework Core can infer the foreign key relationships based on your navigation properties and conventions.</b> However, including the foreign key property can often make your code easier to understand and work with, and it can provide some practical benefits for certain scenarios, as described above.
</blockquote >

<br />

**If you want a smaller structure for serialization, consider DTO's**