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
        
        var newUserId = await _authRepository.Register(user, password);
        return new ServiceResponse<int>
        {
            Data = newUserId
        };
        
    }

    public Task<ServiceResponse<string>> Login(string userName, string password)
    {
        throw new NotImplementedException();
    }
}