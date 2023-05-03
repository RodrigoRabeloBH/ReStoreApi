using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.AutoMock;
using ReStore.Application.Implementations;
using ReStore.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ReStore.Tests.Application
{
    public class TokenServiceTest
    {
        private readonly TokenService _sut;

        private readonly Mock<IConfiguration> _mockConfiguration;

        private readonly Mock<UserManager<User>> _mockUserManager;

        public TokenServiceTest()
        {
            _mockConfiguration = new AutoMocker().GetMock<IConfiguration>();
            _mockUserManager = new AutoMocker().GetMock<UserManager<User>>();

            _sut = new TokenService(_mockConfiguration.Object, _mockUserManager.Object);
        }

        [Fact]
        public async Task GenerateTokenShoulReturnAToken()
        {
            // Arrange
            User user = new()
            {
                Email = "user@test.com",
                UserName = "Test",
            };

            var roles = new List<string> { "Member" };

            _mockUserManager.Setup(x => x.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);

            _mockConfiguration.Setup(x => x["JWTSettings:TokenKey"]).Returns("Super secret token key");

            //Act

            var token = await _sut.GenerateToken(user);

            //Assert

            Assert.NotNull(token);
        }
    }
}
