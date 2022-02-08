using System;
using System.Linq;

using Interfaces;
using Lib;
using Models;
using Models.Requests;
using Models.Responses;

// !!! The User table is intentionally not auditable !!!

namespace Clifton.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAppDbContext context;

        public AccountService(IAppDbContext context)
        {
            this.context = context;
        }

        public LoginResponse Login(AccountRequest req)
        {
            LoginResponse response = null;

            var user = context.User
                .Where(u => u.UserName == req.Username && u.Deleted == false)
                .ToList()   // Because Hasher would otherwise be evaluated in the generated SQL expression.
                .SingleOrDefault(u => Hasher.HashPassword(u.Salt, req.Password) == u.Password);

            if (user != null)
            {
                var ts = GetEpoch();
                user.Login(ts);
                context.SaveChanges();
                response = user.CreateMapped<LoginResponse>();
            }

            return response;
        }

        public LoginResponse Refresh(string refreshToken)
        {
            LoginResponse response = null;

            var user = context.User
                .Where(u => u.RefreshToken == refreshToken && u.Deleted == false).SingleOrDefault();

            if (user != null)
            {
                var ts = GetEpoch();

                // Refresh token expires 90 days after when user logged in, thus ExpiresOn + (90 - 1) days
                if (user.ExpiresOn + (Constants.REFRESH_VALID_DAYS - 1) * Constants.ONE_DAY_IN_SECONDS > ts)
                {
                    user.Login(ts);
                    context.SaveChanges();
                    response = user.CreateMapped<LoginResponse>();
                }
            }

            return response;
        }

        public bool VerifyAccount(string token)
        {
            var user = context.User.Where(u => u.AccessToken == token).SingleOrDefault();
            var ts = GetEpoch();
            bool ok = (user?.ExpiresOn ?? 0) > ts;

            return ok;
        }

        public void Logout(string token)
        {
            var user = context.User.Single(u => u.AccessToken == token);
            user.Logout();
            context.SaveChanges();
        }

        public (bool ok, int id) CreateAccount(AccountRequest req)
        {
            bool ok = false;
            int id = -1;

            var existingUsers = context.User.Where(u => u.UserName == req.Username && !u.Deleted).Count();

            if (existingUsers == 0)
            {
                var salt = Hasher.GenerateSalt();
                var hashedPassword = Hasher.HashPassword(salt, req.Password);
                var user = new User() { UserName = req.Username, Password = hashedPassword, Salt = salt };
                context.User.Add(user);
                context.SaveChanges();
                ok = true;
                id = user.Id;
            }

            return (ok, id);
        }

        public void ChangeUsernameAndPassword(string token, AccountRequest req)
        {
            var user = context.User.Single(u => u.AccessToken == token);
            user.Logout();
            user.Salt = Hasher.GenerateSalt();
            user.UserName = req.Username ?? user.UserName;
            user.Password = Hasher.HashPassword(user.Salt, req.Password);
            context.SaveChanges();
        }

        public void DeleteAccount(string token)
        {
            var user = context.User.Single(u => u.AccessToken == token);
            user.Logout();
            user.Deleted = true;
            context.SaveChanges();
        }

        public User GetUser(string token)
        {
            var user = context.User.SingleOrDefault(u => u.AccessToken == token);

            return user;
        }

        public void ExpireToken(string token)
        {
            var ts = GetEpoch();
            var user = context.User.SingleOrDefault(u => u.AccessToken == token);
            user.ExpiresOn = ts - Constants.ONE_DAY_IN_SECONDS;
            context.SaveChanges();
        }

        public void ExpireRefreshToken(string token)
        {
            var ts = GetEpoch();
            var user = context.User.SingleOrDefault(u => u.AccessToken == token);
            user.ExpiresOn = ts - Constants.REFRESH_VALID_DAYS * Constants.ONE_DAY_IN_SECONDS;
            context.SaveChanges();
        }

        private long GetEpoch()
        {
            var ts = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;   // We declare the epoch to be 1/1/1970.

            return ts;
        }
    }
}