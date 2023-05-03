using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.AutoMock;
using ReStore.Application.Implementations;
using ReStore.Application.Interfaces;
using ReStore.Application.Models;
using ReStore.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ReStore.Tests.Application
{
    public class AccountServicesTests
    {
        private readonly AccountServices _sut;

        private readonly Mock<UserManager<User>> _mockUserManager;

        private readonly Mock<ITokenService> _mockTokenService;

        private readonly Mock<ILogger<AccountServices>> _mockLogger;

        public AccountServicesTests()
        {
            _mockLogger = new AutoMocker().GetMock<ILogger<AccountServices>>();
            _mockTokenService = new AutoMocker().GetMock<ITokenService>();
            _mockUserManager = new AutoMocker().GetMock<UserManager<User>>();

            _sut = new AccountServices(_mockUserManager.Object, _mockTokenService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task LoginShouldReturnUserModelWithToken()
        {
            // Arrange

            string token = "Super secret token";

            UserLoginModel userLoginModel = new() { Password = "Pa$w0rd", UserName = "John Doe" };

            User user = new() { UserName = "John Doe", Email = "john@test.com" };

            _mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);

            _mockUserManager.Setup(x => x.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(true);

            _mockTokenService.Setup(x => x.GenerateToken(It.IsAny<User>())).ReturnsAsync(token);

            //Act

            var result = await _sut.Login(userLoginModel);

            //Assert

            Assert.NotNull(result);
            Assert.Equal(token, result.Token);
            Assert.Equal(user.UserName, result.UserName);
            Assert.Equal(user.Email, result.Email);

            VerifyLogTest("[Logging in user]");
            VerifyLogTest("[User logged successfuly]");
        }

        [Fact]
        public async Task LoginShouldReturnNullAndLogMessageUnauthorized()
        {
            // Arrange

            UserLoginModel userLoginModel = new() { Password = "Pa$w0rd", UserName = "John Doe" };

            User user = new() { UserName = "John Doe", Email = "john@test.com" };

            _mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);

            _mockUserManager.Setup(x => x.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(false);

            //Act

            var result = await _sut.Login(userLoginModel);

            //Assert

            Assert.Null(result);
            VerifyLogTest("[Logging in user]");
            VerifyLogTest("[User unauthorized]");
        }

        [Fact]
        public async Task RegisterShouldReturnSucceededIdentityResult()
        {
            // Arrange

            UserRegisterModel userRegisterModel = new()
            {
                Email = "jonh@test.com",
                Password = "password",
                UserName = "Jonh Doe"
            };

            User user = new() { UserName = userRegisterModel.UserName, Email = userRegisterModel.Email };

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()));

            //Act

            var result = await _sut.Register(userRegisterModel);

            //Assert

            Assert.True(result.Succeeded);
            VerifyLogTest("[Registering user]");
            VerifyLogTest("[User registered successfuly]");
            _mockUserManager.Verify(x => x.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task RegisterShouldReturnFailedIdentityResult()
        {
            // Arrange

            UserRegisterModel userRegisterModel = new()
            {
                Email = "jonh@test.com",
                Password = "password",
                UserName = "Jonh Doe"
            };

            User user = new() { UserName = userRegisterModel.UserName, Email = userRegisterModel.Email };

            var identityResult = IdentityResult.Failed(
               new IdentityError[] {
                  new IdentityError{
                     Code = "0001",
                     Description = "Teste Error 001"
                  },
                  new IdentityError{
                     Code = "0002",
                     Description = "Teste Error 002"
                  }
               }
            );

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(identityResult);

            //Act

            var result = await _sut.Register(userRegisterModel);

            //Assert

            VerifyLogTest("[Registering user]");
            Assert.False(result.Succeeded);
            Assert.Equal(2, result.Errors.Count());
            Assert.Equal(identityResult.Errors.First().Code, result.Errors.First().Code);
        }

        [Fact]
        public async Task GetCurrentUserShouldReturnCurrentUser()
        {
            // Arrange

            string name = "John Doe";

            User user = new() { UserName = name };

            _mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);

            //Act

            var result = await _sut.GetCurrentUser(name);

            //Assert

            Assert.Equal(name, result.UserName);
        }

        private void VerifyLogTest(string message, bool timesOnce = true)
        {

            _mockLogger.Verify(
               x => x.Log(
                   LogLevel.Information,
                   It.IsAny<EventId>(),
                   It.Is<It.IsAnyType>((o, t) => string.Equals(message, o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
                   It.IsAny<Exception>(),
                   It.IsAny<Func<It.IsAnyType, Exception, string>>()), timesOnce ? Times.Once : Times.Never);
        }
    }
}
