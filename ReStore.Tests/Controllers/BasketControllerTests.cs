using API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using ReStore.Application.Models;
using ReStore.Applicationfaces;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ReStore.Tests.Controllers
{
    public class BasketControllerTests
    {
        private readonly BasketController _sut;
        private readonly Mock<IStoreServices> _mockService;
        private readonly string buyerId = Guid.NewGuid().ToString();

        public BasketControllerTests()
        {
            _mockService = new AutoMocker().GetMock<IStoreServices>();

            _sut = new BasketController(_mockService.Object);
        }

        [Fact]
        public async Task GetBasketShouldReturnActionResultTypeOfBasketModel()
        {
            // Arrange

            var basket = GenerateBasketModel(buyerId);

            _mockService.Setup(x => x.GetBasketByBuyerId(It.IsAny<string>())).ReturnsAsync(basket);

            //Act 

            var actionResult = await _sut.GetBasket();

            //Assert

            Assert.IsType<BasketModel>(actionResult.Value);
            Assert.Equal(buyerId, actionResult.Value.BuyerId);
        }

        [Fact]
        public async Task GetBasketShouldReturnNotFound()
        {
            // Arrange           

            _mockService.Setup(x => x.GetBasketByBuyerId(It.IsAny<string>())).ReturnsAsync(value: null);

            //Act 

            var actionResult = await _sut.GetBasket();

            //Assert

            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task RemoveItemFromBasketShoudReturnStatusCode200()
        {
            // Arrange

            int productId = 10;

            int quantity = 5;

            _mockService.Setup(x => x.RemoveItemFromBasket(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(true);

            //Act 

            var actionResult = await _sut.RemoveItemFromBasket(productId, quantity);

            //Assert

            var okResult = actionResult as OkResult;

            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task RemoveItemFromBasketShoudReturnBadRequest()
        {
            // Arrange

            int productId = 10;

            int quantity = 5;

            _mockService.Setup(x => x.RemoveItemFromBasket(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(false);

            //Act 

            var actionResult = await _sut.RemoveItemFromBasket(productId, quantity);

            var badRequestObjectResult = actionResult as BadRequestObjectResult;

            var problemDetails = badRequestObjectResult.Value as ProblemDetails;

            //Assert

            Assert.Equal(400, badRequestObjectResult.StatusCode);

            Assert.Equal("Problem removing item from the basket", problemDetails.Title);
        }

        [Fact]
        public async Task AddItemToBasketShouldReturnCreatedAtRouteResult()
        {
            // Arrange

            int productId = 10;

            int quantity = 5;

            var basket = GenerateBasketModel(buyerId);

            _mockService.Setup(x => x.GetBasketByBuyerId(It.IsAny<string>())).ReturnsAsync(basket);

            _mockService.Setup(x => x.AddItemToBasket(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(basket);

            //Act

            var result = await _sut.AddItemToBasket(productId, quantity);

            var routeResult = result.Result as CreatedAtRouteResult;

            //Assert

            Assert.IsType<CreatedAtRouteResult>(result.Result);

            Assert.Equal(201, routeResult.StatusCode);
        }

        [Fact]
        public async Task AddItemToBasketShouldReturnBadRequest()
        {
            // Arrange

            int productId = 10;

            int quantity = 5;

            _mockService.Setup(x => x.GetBasketByBuyerId(It.IsAny<string>())).ReturnsAsync(value: null);

            _mockService.Setup(x => x.AddItemToBasket(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(value: null);

            //Act

            var actionResult = await _sut.AddItemToBasket(productId, quantity);

            var badRequestObjectResult = actionResult.Result as BadRequestObjectResult;

            var problemDetails = badRequestObjectResult.Value as ProblemDetails;

            //Assert

            Assert.Equal(400, badRequestObjectResult.StatusCode);

            Assert.Equal("Problem saving to basket", problemDetails.Title);
        }

        private BasketModel GenerateBasketModel(string buyerId)
        {
            return new BasketModel
            {
                Id = 1,
                BuyerId = buyerId,
                Items = { }
            };
        }
    }
}
