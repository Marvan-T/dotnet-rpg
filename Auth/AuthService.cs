namespace dotnet_rpg.Auth;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;

    public AuthService(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }
    
    public async Task<ServiceResponse<int>> Register(User user, string password)
    {
        if (await _authRepository.UserExists(user.Username))
        {
            return new ServiceResponse<int>
            {
                Success = false,
                Message = $"User with the username: {user.Username} already exists"
            };
        }
        
        var newUserId = await _authRepository.CreateUser(user, password);
        return new ServiceResponse<int>
        {
            Data = newUserId
        };
        
    }

    public async Task<ServiceResponse<string>> Login(string userName, string password)
    {
        var response = new ServiceResponse<string>();
        var user = await _authRepository.GetUser(userName);
        
        if (user is null || !VerifyPassswordHash(password, user.PasswordHash, user.PasswordSalt))
        {
            response.Success = false;
            response.Message = "Invalid usernmae or password";
        }
        else
        {
            response.Message = user.Id.ToString();
        }

        return response;
    }

    private bool VerifyPassswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt);
        var computedHash = hmac.ComputeHash((System.Text.Encoding.UTF8.GetBytes(password)));
        return computedHash.SequenceEqual(passwordHash);
    }

}