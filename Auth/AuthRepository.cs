using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_rpg.Auth;

public class AuthRepository : IAuthRepository
{
    private readonly DataContext _context;

    public AuthRepository(DataContext context)
    {
        _context = context;
    }

    public Task<string> Login(string userName, string password)
    {
        throw new NotImplementedException();
    }

    public async Task<int> Register(User user, string password)
    {
        //By declaring passwordHash and passwordSalt as out parameters,
        //we're asking the CreatePasswordHash method to initialize and set these variables
        //Using the out keyword tells the compiler that the variables will be assigned a value by the method and that their initial values can be ignored
        CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user.Id;
    }

    public async Task<bool> UserExists(string userName)
    {
        if (await _context.Users.AnyAsync(u => u.Username.ToLower() == userName.ToLower()))
        {
            return true;
        }
        return false;
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        /*
         out
            - allows you to return multiple values from a method
            - it will change the value of the passed argument and the change will be reflected back in the calling code
        */
        using var hmac = new System.Security.Cryptography.HMACSHA512();
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
    }
}