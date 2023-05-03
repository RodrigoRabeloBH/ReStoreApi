using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using ReStore.Application.Implementations;
using ReStore.Application.Interfaces;
using ReStore.Application.Models;
using ReStore.Domain.Entities;
using ReStore.Domain.Interfaces;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ReStore.Tests.Application
{
    public class StoreServicesTest
    {
        private readonly StoreServices _sut;
        private readonly Mock<IStoreRepository> _repositoryMock;
        private readonly Mock<ILogger<StoreServices>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IPaymentService> _paymentMock;
        private readonly Mock<IMailService> _mockMailServices;
        public StoreServicesTest()
        {
            _repositoryMock = new Mock<IStoreRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<StoreServices>>();
            _paymentMock = new Mock<IPaymentService>();
            _mockMailServices = new Mock<IMailService>();

            _sut = new StoreServices(_repositoryMock.Object, _loggerMock.Object, _mapperMock.Object, _paymentMock.Object, _mockMailServices.Object);
        }

        [Fact]
        public async Task GetProductByIdShouldReturnOneProduct()
        {
            // Arrange

            var product = new Product
            {
                Brand = "brand",
                Description = "Product created for unit test one",
                Id = 1,
                Name = "Beterraba",
                Price = 50
            };

            //Act

            _repositoryMock.Setup(x => x.GetProductById(It.IsAny<int>())).ReturnsAsync(product);

            var result = await _sut.GetProductById(50);

            //Assert

            Assert.NotNull(result);
            Assert.Equal("Beterraba", result.Name);
            Assert.Equal(1, result.Id);
            Assert.Equal(50, result.Price);

            VerifyLogTest("[Getting product by id]");

            VerifyLogTest($"Product: {product.Name}");
        }

        [Fact]
        public async Task GetProductByIdShouldReturnNullWhenProductIdIsNotFound()
        {
            // Arrange    

            Product product = new() { Name = "Banana" };

            //Act

            _repositoryMock.Setup(x => x.GetProductById(It.IsAny<int>())).ReturnsAsync(value: null);

            var result = await _sut.GetProductById(50);

            //Assert

            Assert.Null(result);

            VerifyLogTest("[Getting product by id]");

            VerifyLogTest($"Product: {product.Name}", false);
        }

        [Fact]
        public async Task GetBasketByBuyerIdShouldReturnABasket()
        {
            // Arrange

            string buyerId = Guid.NewGuid().ToString();

            BasketModel basketModelMock = new() { BuyerId = buyerId };

            Basket basketMock = new()
            {
                BuyerId = buyerId,
                Id = 69,
                Items = { }
            };

            _repositoryMock.Setup(x => x.RetrieveBasket(It.IsAny<string>())).ReturnsAsync(basketMock);

            _mapperMock.Setup(x => x.Map<BasketModel>(It.IsAny<Basket>())).Returns(basketModelMock);

            //Act

            var result = await _sut.GetBasketByBuyerId(buyerId);

            //Assert

            Assert.NotNull(result);
            Assert.Equal(basketModelMock.BuyerId, result.BuyerId);

            VerifyLogTest("[Getting basket for buyerid: " + buyerId + "]");
        }

        [Fact]
        public async Task GetBasketByBuyerIdShouldNotReturnABasketWhenNotFountABasketWithTheSameBuyerId()
        {
            // Arrange

            string buyerId = Guid.NewGuid().ToString();

            _repositoryMock.Setup(x => x.RetrieveBasket(It.IsAny<string>())).ReturnsAsync(value: null);

            _mapperMock.Setup(x => x.Map<BasketModel>(It.IsAny<Basket>())).Returns(value: null);

            //Act

            var result = await _sut.GetBasketByBuyerId(buyerId);

            //Assert

            Assert.Null(result);

            VerifyLogTest("[Getting basket for buyerid: " + buyerId + "]");
        }

        [Fact]
        public async Task CreateBasketShouldBeCalled()
        {
            // Arrange

            string buyerId = Guid.NewGuid().ToString();

            BasketModel basketModel = new() { BuyerId = buyerId };

            Basket basket = new() { BuyerId = buyerId };

            _mapperMock.Setup(x => x.Map<Basket>(It.IsAny<BasketModel>())).Returns(basket);

            _repositoryMock.Setup(x => x.CreateBasket(It.IsAny<Basket>()));

            //Act 

            await _sut.CreateBasket(basketModel);

            //Assert

            _repositoryMock.Verify(x => x.CreateBasket(basket), Times.Once);

            VerifyLogTest("[Creating basket ...]");
        }

        [Fact]
        public async Task AddItemToBasketShouldAddAnItemToBasketWhenBasketExists()
        {
            // Arrange

            int productId = 1; int quantity = 10; string buyerId = Guid.NewGuid().ToString();

            Basket basket = new() { BuyerId = buyerId };
            Product product = new() { Name = "Banana", Id = 10 };
            BasketModel basketModel = new() { BuyerId = buyerId };

            _repositoryMock.Setup(x => x.RetrieveBasket(It.IsAny<string>())).ReturnsAsync(basket);
            _repositoryMock.Setup(x => x.GetProductById(It.IsAny<int>())).ReturnsAsync(product);
            _repositoryMock.Setup(x => x.SaveChanges());
            _mapperMock.Setup(x => x.Map<BasketModel>(It.IsAny<Basket>())).Returns(basketModel);

            //Act 

            var result = await _sut.AddItemToBasket(productId, quantity, buyerId);

            //Assert

            Assert.NotNull(result);
            Assert.Equal(buyerId, result.BuyerId);

            _repositoryMock.Verify(x => x.SaveChanges(), Times.Once);
            _repositoryMock.Verify(x => x.GetProductById(productId), Times.Once);
            _repositoryMock.Verify(x => x.RetrieveBasket(buyerId), Times.Once);

            VerifyLogTest("[Adding item to basket ...]");
        }

        [Fact]
        public async Task AddItemToBasketShouldCreatABasketAndAddAnItemToBasketWhenBasketNotExists()
        {
            // Arrange

            int productId = 1; int quantity = 10; string buyerId = Guid.NewGuid().ToString();

            Basket basket = null;
            Product product = new() { Name = "Banana", Id = 10 };
            BasketModel basketModel = new() { BuyerId = buyerId };

            _repositoryMock.Setup(x => x.RetrieveBasket(It.IsAny<string>())).ReturnsAsync(basket);
            _repositoryMock.Setup(x => x.GetProductById(It.IsAny<int>())).ReturnsAsync(product);
            _repositoryMock.Setup(x => x.CreateBasket(It.IsAny<Basket>()));
            _repositoryMock.Setup(x => x.SaveChanges());
            _mapperMock.Setup(x => x.Map<BasketModel>(It.IsAny<Basket>())).Returns(basketModel);

            //Act 

            var result = await _sut.AddItemToBasket(productId, quantity, buyerId);

            //Assert

            Assert.NotNull(result);
            Assert.Equal(buyerId, result.BuyerId);

            _repositoryMock.Verify(x => x.SaveChanges(), Times.Once);
            _repositoryMock.Verify(x => x.GetProductById(productId), Times.Once);
            _repositoryMock.Verify(x => x.RetrieveBasket(buyerId), Times.Once);
            _repositoryMock.Verify(x => x.CreateBasket(It.IsAny<Basket>()), Times.Once);

            VerifyLogTest("[Adding item to basket ...]");
        }

        [Fact]
        public async Task AddItemToBasketShouldReturnNullWhenProductAddedNotExists()
        {
            // Arrange

            int productId = 1; int quantity = 10; string buyerId = Guid.NewGuid().ToString();

            Basket basket = null;
            Product product = null;
            BasketModel basketModel = new() { BuyerId = buyerId };

            _repositoryMock.Setup(x => x.RetrieveBasket(It.IsAny<string>())).ReturnsAsync(basket);
            _repositoryMock.Setup(x => x.GetProductById(It.IsAny<int>())).ReturnsAsync(product);
            _repositoryMock.Setup(x => x.CreateBasket(It.IsAny<Basket>()));
            _repositoryMock.Setup(x => x.SaveChanges());
            _mapperMock.Setup(x => x.Map<BasketModel>(It.IsAny<Basket>())).Returns(basketModel);

            //Act 

            var result = await _sut.AddItemToBasket(productId, quantity, buyerId);

            //Assert

            Assert.Null(result);

            _repositoryMock.Verify(x => x.SaveChanges(), Times.Never);
            _repositoryMock.Verify(x => x.GetProductById(productId), Times.Once);
            _repositoryMock.Verify(x => x.RetrieveBasket(buyerId), Times.Once);
            _repositoryMock.Verify(x => x.CreateBasket(It.IsAny<Basket>()), Times.Once);

            VerifyLogTest("[Adding item to basket ...]");
        }

        [Fact]
        public async Task RemoveItemFromBasketShouldBeCalled()
        {
            // Arrange

            int productId = 1;
            int quantity = 10;
            string buyerId = Guid.NewGuid().ToString();

            _repositoryMock.Setup(x => x.RemoveItemFromBasket(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

            //Act 

            await _sut.RemoveItemFromBasket(productId, quantity, buyerId);

            //Assert

            _repositoryMock.Verify(x => x.RemoveItemFromBasket(productId, quantity, buyerId), Times.Once);

            VerifyLogTest("[Removing item from basket ...]");
        }

        private void VerifyLogTest(string message, bool timesOnce = true)
        {

            _loggerMock.Verify(
               x => x.Log(
                   LogLevel.Information,
                   It.IsAny<EventId>(),
                   It.Is<It.IsAnyType>((o, t) => string.Equals(message, o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
                   It.IsAny<Exception>(),
                   It.IsAny<Func<It.IsAnyType, Exception, string>>()), timesOnce ? Times.Once : Times.Never);
        }
    }
}
