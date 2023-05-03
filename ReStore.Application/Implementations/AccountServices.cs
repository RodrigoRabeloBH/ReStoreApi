using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ReStore.Application.Interfaces;
using ReStore.Application.Models;
using ReStore.Domain.Entities;
using ReStore.Domain.Enum;
using System.Threading.Tasks;

namespace ReStore.Application.Implementations
{
    public class AccountServices : IAccountServices
    {
        private readonly UserManager<User> _userManager;

        private readonly ITokenService _tokenService;

        private readonly ILogger<AccountServices> _logger;

        public AccountServices(UserManager<User> userManager, ITokenService tokenService, ILogger<AccountServices> logger)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<UserModel> Login(UserLoginModel userLoginModel)
        {
            _logger.LogInformation("[Logging in user]");

            var user = await _userManager.FindByNameAsync(userLoginModel.UserName);

            if (user == null || !await _userManager.CheckPasswordAsync(user, userLoginModel.Password))
            {
                _logger.LogInformation("[User unauthorized]");

                return null;
            }

            _logger.LogInformation("[User logged successfuly]");

            return new UserModel
            {
                Email = user.Email,
                UserName = user.UserName,
                Token = await _tokenService.GenerateToken(user)
            };
        }

        public async Task<IdentityResult> Register(UserRegisterModel userRegisterModel)
        {
            _logger.LogInformation("[Registering user]");

            var user = new User { UserName = userRegisterModel.UserName, Email = userRegisterModel.Email };

            var result = await _userManager.CreateAsync(user, userRegisterModel.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, AccessLevel.Member.ToString());

                _logger.LogInformation("[User registered successfuly]");
            }

            return result;
        }

        public async Task<UserModel> GetCurrentUser(string name)
        {
            var user = await _userManager.FindByNameAsync(name);

            return new UserModel
            {
                Email = user.Email,
                UserName = user.UserName,
                Token = await _tokenService.GenerateToken(user)
            };
        }
    }
}

