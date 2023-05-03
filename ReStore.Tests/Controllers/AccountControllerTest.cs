using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using ReStore.Api.Controllers;
using ReStore.Application.Interfaces;
using ReStore.Application.Models;
using ReStore.Domain.Entities;
using ReStore.Infrastructure.Data;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ReStore.Tests.Controllers
{
    public class AccountControllerTest
    {
        private readonly AccountController _sut;
        private readonly Mock<IAccountServices> _mockAccountServices;
        private readonly Mock<StoreContext> _mockContext;
        public AccountControllerTest()
        {
            _mockContext = new Mock<StoreContext>();
            _mockAccountServices = new Mock<IAccountServices>();

            _sut = new AccountController(_mockAccountServices.Object, _mockContext.Object);
        }

        [Fact]
        public async Task LoginShouldReturnUAuthorized()
        {
            // Arrange

            UserLoginModel userLoginModel = new() { Password = "password", UserName = "John Doe" };

            UserModel userModel = null;

            _mockAccountServices.Setup(x => x.Login(It.IsAny<UserLoginModel>())).ReturnsAsync(userModel);

            //Act

            var actionResult = await _sut.Login(userLoginModel);

            //Assert

            Assert.IsType<UnauthorizedResult>(actionResult.Result);
        }

        [Fact]
        public async Task RegisterShouldReturnStatusCode201()
        {
            //Arrange

            UserRegisterModel userRegisterModel = GenerateUserRegisterModel();

            _mockAccountServices.Setup(x => x.Register(It.IsAny<UserRegisterModel>())).ReturnsAsync(IdentityResult.Success);

            //Act

            var actionResult = await _sut.Register(userRegisterModel);

            //Assert

            var statusCodeResult = actionResult as StatusCodeResult;

            Assert.Equal(201, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task RegisterShouldReturnValidationProblemDetails()
        {
            // Arrange 

            UserRegisterModel userRegisterModel = GenerateUserRegisterModel();

            var identityResult = IdentityResult
                .Failed(new IdentityError[]
                {
                    new IdentityError { Code = "0001", Description = "Teste Error 001" },
                    new IdentityError { Code = "0002", Description = "Teste Error 002" }
                });

            _mockAccountServices.Setup(x => x.Register(It.IsAny<UserRegisterModel>())).ReturnsAsync(identityResult);

            //Act

            var actionResult = await _sut.Register(userRegisterModel);

            // Assert

            var objectResult = actionResult as ObjectResult;

            Assert.IsType<ValidationProblemDetails>(objectResult.Value);
        }

        [Fact]
        public async Task GetCurrentUserShoudReturnActionResultTypeOfUserModel()
        {
            //Arrange

            var userModel = new UserModel
            {
                Email = "john@test.com",
                UserName = "John Doe",
                Token = "Super secret token"
            };

            _mockAccountServices.Setup(x => x.GetCurrentUser(It.IsAny<string>())).ReturnsAsync(userModel);

            //Act

            var actionResult = await _sut.GetCurrentUser();

            //Assert
            Assert.IsType<UserModel>(actionResult.Value);
            Assert.Equal(userModel.Email, actionResult.Value.Email);
            Assert.Equal(userModel.UserName, actionResult.Value.UserName);
            Assert.Equal(userModel.Token, actionResult.Value.Token);
        }

        private UserRegisterModel GenerateUserRegisterModel()
        {
            UserRegisterModel userRegisterModel = new UserRegisterModel()
            {
                Email = "john@test.com",
                Password = "Pa$$w0rd",
                UserName = "John Doe"
            };

            return userRegisterModel;
        }
    }
}
