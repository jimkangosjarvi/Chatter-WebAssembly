using Chatter.Shared.Models.Register;
using Chatter.Shared.Models.Login;
using System.Threading.Tasks;

namespace Chatter.Client.Services
{
    public interface IAuthService
    {
        Task<LoginResult> Login(LoginModel loginModel);
        Task Logout();
        Task<RegisterResult> Register(RegisterModel registerModel);
    }
}
