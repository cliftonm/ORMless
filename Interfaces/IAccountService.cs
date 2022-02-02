using Models.Requests;
using Models.Responses;

namespace Interfaces
{
    public interface IAccountService
    {
        LoginResponse Login(AccountRequest req);
        bool CreateAccount(AccountRequest req);
        bool VerifyAccount(string token);
        void Logout(string token);
        void DeleteAccount(string token);
        void ChangeUsernameAndPassword(string token, AccountRequest req);
    }
}
