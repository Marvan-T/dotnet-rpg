# To set up we need the following packages within the project

1. Microsoft.EntityFrameworkCore https://www.nuget.org/packages/Microsoft.EntityFrameworkCore

    - This is the primary package for Entity Framework Core, which is a popular Object-Relational Mapping (ORM) tool in .NET. It helps you to interact with your database using .NET objects. It includes APIs for database operations like querying, inserting, updating and deleting data.

    ```shell
    dotnet add package Microsoft.EntityFrameworkCore 
    ```

2. Microsoft.EntityFrameworkCore.Design
    https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Design
    - This package contains shared design-time components for Entity Framework Core tools. Basically, for working with migrations (similar to Flyway)

     ```shell
    dotnet add package Microsoft.EntityFrameworkCore.Design 
    ```

3. Database Drivers
    - For the tutorial we use SQL Server
    https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.SqlServer

    - database provider that allows Entity Framework Core to work with Microsoft SQL Serve

    ```shell
    dotnet add package Microsoft.EntityFrameworkCore.SqlServer
    ```

4. Install `dotnet tool install --global dotnet-ef`
    - This is a command-line tool that you can use for Entity Framework Core development. You can use it to create migrations, apply migrations, create a database schema from your model, and execute other development tasks. It's not a NuGet package, but a .NET Global Tool that you install on your machine.

    - I think we have to install this only once

    - After installation you can interact with this with by `dotnet ef`

5. Setting up the `DB Context` and `Connection Strings`
    - Look at classes `DataContext.cs`, `appsettings.json` and `Program.cs` on how to configure the DB
    
<br />

6. Once the above is complete you can use the command `dotnet ef -h` to see the available commands
    - database   Commands to manage the database.
    - dbcontext   Commands to manage DbContext types.
    - migrations  Commands to manage migrations.

<br />

7. Creating and running migrations 
    - To create the first migration you can do `dotnet ef migrations add InitialCreate`, here `InitialCreate` is the name of the migration (creates new migrations based on changes in your data model)
    - To apply the migrations to the database use the command `dotnet ef database update`

