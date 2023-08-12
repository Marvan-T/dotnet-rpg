using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace dotnet_rpg.Auth;

public class AuthRepository : IAuthRepository
{
    private readonly DataContext _context;
    private readonly IHttpContextAccessor _iHttpContextAccessor;

    public AuthRepository(DataContext context, IHttpContextAccessor iHttpContextAccessor)
    {
        _context = context;
        _iHttpContextAccessor = iHttpContextAccessor;
    }

    public async Task<User> CreateUser(User user, string password)
    {
        //By declaring passwordHash and passwordSalt as out parameters,
        //we're asking the CreatePasswordHash method to initialize and set these variables
        //Using the out keyword tells the compiler that the variables will be assigned a value by the method and that their initial values can be ignored
        CreatePasswordHash(password, out var passwordHash, out var passwordSalt);
        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> UserExists(string userName)
    {
        return await _context.Users.AnyAsync(u =>
            string.Equals(u.Username, userName, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<User> GetUser(string userName)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username.Equals(userName));
    }

    //Update later
    public async Task<User> GetByIdAsync(int id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public int GetCurrentUserId()
    {
        return int.Parse(_iHttpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        /*
         out
            - allows you to return multiple values from a method
            - it will change the value of the passed argument and the change will be reflected back in the calling code
        */

        // When the HMACSHA512 class is initialized, it automatically generates a new Key if one isn't provided.
        // This Key is a random set of bytes that is then used as passwordSalt. 
        using var hmac = new HMACSHA512();
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }
}