using Microsoft.AspNetCore.Identity;
using ReStore.Application.Models;
using System.Threading.Tasks;

namespace ReStore.Application.Interfaces
{
    public interface IAccountServices
    {
        Task<IdentityResult> Register(UserRegisterModel userRegisterModel);

        Task<UserModel> Login(UserLoginModel userLoginModel);

        Task<UserModel> GetCurrentUser(string name);
    }
}
