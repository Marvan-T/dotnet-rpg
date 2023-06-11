# To set up we need the following packages within the project

1. Microsoft.EntityFrameworkCore <br />
    https://www.nuget.org/packages/Microsoft.EntityFrameworkCore

    - This is the primary package for Entity Framework Core, which is a popular Object-Relational Mapping (ORM) tool in .NET. It helps you to interact with your database using .NET objects. It includes APIs for database operations like querying, inserting, updating and deleting data.
    
    ```shell
    dotnet add package Microsoft.EntityFrameworkCore 
    ```
<br/>

2. Microsoft.EntityFrameworkCore.Design
https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Design
    - This package contains shared design-time components for Entity Framework Core tools. Basically, for working with migrations (similar to Flyway)
     ```shell
    dotnet add package Microsoft.EntityFrameworkCore.Design 
    ```

<br/>

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