using Models;
using Models.Requests;
using Models.Responses;

namespace Interfaces
{
    public interface IAccountService
    {
        LoginResponse Login(AccountRequest req);
        LoginResponse Refresh(string refreshToken);
        (bool ok, int id) CreateAccount(AccountRequest req);
        bool VerifyAccount(string token);
        User GetUser(string token);
        void Logout(string token);
        void DeleteAccount(string token);
        void ChangeUsernameAndPassword(string token, AccountRequest req);
    }
}
