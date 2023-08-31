namespace dotnet_rpg.Auth;

public interface IAuthRepository
{
    Task<User> CreateUser(User user, string password);
    Task<bool> UserExists(string userName);
    Task<User> GetUser(string userName);
    Task<User> GetByIdAsync(int id);
    int GetCurrentUserId();
}