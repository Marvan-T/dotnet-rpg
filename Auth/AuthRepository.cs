using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_rpg.Auth
{
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

        public Task<int> Register(User user, string password)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UserExists(string userName)
        {
            throw new NotImplementedException();
        }
    }
}