using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_rpg.Auth;

public interface IAuthService
{
    Task<ServiceResponse<string>> Register(User user, string password);

    Task<ServiceResponse<string>> Login(string userName, string password);
    //UserExist is omitted 
}