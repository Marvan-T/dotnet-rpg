using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_rpg.Data;

/*
  You can use an instance of DbContext to interact with the database (query)
*/
public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
            
    }

    /* 
    Name of the DbSet is the name of the corresponding database table. 
    When you need a representation of a model in the database, you have to add a DbSet of the model.

    This is how it knows what tables that it should create 
    */
    //a property with only a getter, which uses the Set<TEntity> method to get a DbSet for Character entities. - Read Demystifying DataContext
    public DbSet<Character> Characters => Set<Character>(); 
    public DbSet<User> Users => Set<User>();
    public DbSet<Weapon> Weapons => Set<Weapon>();

}