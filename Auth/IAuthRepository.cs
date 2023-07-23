using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_rpg.Auth;

public interface IAuthRepository
{
    Task<int> Register(User user, string password);
    Task<bool> UserExists(string userName);
    Task<User> GetUser(string userName);
}