using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace dotnet_rpg.Auth;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IAuthRepository authRepository, IConfiguration configuration)
    {
        _authRepository = authRepository;
        _configuration = configuration;
    }
    
    public async Task<ServiceResponse<string>> Register(User user, string password)
    {
        if (await _authRepository.UserExists(user.Username))
        {
            return new ServiceResponse<string>
            {
                Success = false,
                Message = $"User with the username: {user.Username} already exists"
            };
        }
        
        var createdUser = await _authRepository.CreateUser(user, password);
        var token = CreateToken(createdUser);
        return new ServiceResponse<string>
        {
            Data = token
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
            response.Data = CreateToken(user);
        }

        return response;
    }

    private bool VerifyPassswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt);
        var computedHash = hmac.ComputeHash((System.Text.Encoding.UTF8.GetBytes(password)));
        return computedHash.SequenceEqual(passwordHash);
    }

    private string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };

        //Singing key is used to create a Symmetric Security Key, this is used to sign the JWT token
        var JwtSigningKey = _configuration.GetSection("AppSettings:JwtSigningKey").Value;

        if (JwtSigningKey is null)
            throw new Exception("JWT signing is not provided");

        SymmetricSecurityKey key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(JwtSigningKey));
        SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        //Token descriptior contains the details about the token (claims, expiration & signing credentials)
        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject =  new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(1),
            SigningCredentials = creds
        };

        //TokenHandler is used to create the actual token based on the details on the SecurityTokenDescriptor
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

}