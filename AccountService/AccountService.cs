using System;
using System.Linq;

using Interfaces;
using Models;
using Models.Requests;
using Models.Responses;

namespace Clifton.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAppDbContext context;

        public AccountService(IAppDbContext context)
        {
            this.context = context;
        }

        public LoginResponse Login(LoginRequest req)
        {
            LoginResponse response = null;

            var user = context.User
                .Where(u => u.UserName == req.Username && u.Deleted == false)
                .ToList()   // Because Hasher would otherwise be evaluated in the generated SQL expression.
                .SingleOrDefault(u => Hasher.HashPassword(u.Salt, req.Password) == u.Password);

            if (user != null)
            {
                var ts = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;   // We declare the epoch to be 1/1/1970.
                user.AccessToken = Guid.NewGuid().ToString();
                user.RefreshToken = Guid.NewGuid().ToString();  // Valid for 90 days
                user.ExpiresIn = 24 * 60 * 60;
                user.ExpiresOn = ts + user.ExpiresIn;
                user.LastLogin = DateTime.Now;
                context.SaveChanges();
                response = user.CreateMapped<LoginResponse>();
            }

            return response;
        }

        public bool CreateAccount(LoginRequest req)
        {
            bool ok = false;
            var existingUsers = context.User.Where(u => u.UserName == req.Username && !u.Deleted).Count();

            if (existingUsers == 0)
            {
                var salt = Hasher.GenerateSalt();
                var hashedPassword = Hasher.HashPassword(salt, req.Password);
                var user = new User() { UserName = req.Username, Password = hashedPassword, Salt = salt };
                context.User.Add(user);
                context.SaveChanges();
                ok = true; 
            }

            return ok;
        }
    }
}