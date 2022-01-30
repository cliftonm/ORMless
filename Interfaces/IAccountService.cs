using Models.Requests;
using Models.Responses;

namespace Interfaces
{
    public interface IAccountService
    {
        LoginResponse Login(LoginRequest req);
        bool CreateAccount(LoginRequest req);
    }
}
