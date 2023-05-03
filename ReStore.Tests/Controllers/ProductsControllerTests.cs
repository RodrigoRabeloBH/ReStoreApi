using API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using ReStore.Application.RequestHelpers;
using ReStore.Applicationfaces;
using ReStore.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ReStore.Tests.Controllers
{
    public class ProductsControllerTests
    {
        private readonly ProductsController _sut;
        private readonly Mock<IStoreServices> _mockService = new AutoMocker().GetMock<IStoreServices>();

        public ProductsControllerTests()
        {
            _sut = new ProductsController(_mockService.Object);
        }

        [Fact]
        public async Task GetProductsShoutReturnActionResultTypeOfPagedList()
        {
            // Arrange

            ProductParams productParams = new ProductParams
            {
                Brands = "Angular",
                OrderBy = "price",
                PageNumber = 1,
                PageSize = 10,
                SearchTerm = "banana",
                Types = "types"
            };

            var items = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "Test"
                }
            };

            PagedList<Product> products = new PagedList<Product>(items, 100, 1, 10);

            _mockService.Setup(x => x.GetProducts(It.IsAny<ProductParams>())).ReturnsAsync(products);

            //Act

            var actionResult = await _sut.GetProducts(productParams);

            //Assert

            Assert.IsType<PagedList<Product>>(actionResult.Value);
            Assert.Equal(items[0].Id, actionResult.Value[0].Id);
        }

        [Fact]
        public async Task GetProductByIdShouldReturnProduct()
        {
            // Arrange

            int productId = 10;

            var product = new Product { Brand = "Angular", Id = productId };

            _mockService.Setup(x => x.GetProductById(It.IsAny<int>())).ReturnsAsync(product);

            //Act 

            var result = await _sut.GetProduct(productId);

            //Assert

            Assert.IsType<Product>(result.Value);
            Assert.Equal(product.Brand, result.Value.Brand);
        }

        [Fact]
        public async Task GetProductByIdShouldReturnNotFound()
        {
            // Arrange

            int productId = 10;

            Product product = null;

            _mockService.Setup(x => x.GetProductById(It.IsAny<int>())).ReturnsAsync(product);

            //Act 

            var actionResult = await _sut.GetProduct(productId);

            //Assert 

            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task GetFiltersShouldReturnTypesAndBrandsAndOkObjectResult()
        {
            // Arrange

            var productFilter = new ProductFilter
            {
                Types = new List<string> { "Boots", "Hats" },
                Brands = new List<string> { "Angular", "React" }
            };


            _mockService.Setup(x => x.GetBrandsAndTypes()).ReturnsAsync(productFilter);

            //Act 

            var result = await _sut.GetFilters();

            //Assert       

            Assert.IsType<ProductFilter>(result.Value);
            Assert.Equal(2, result.Value.Brands.Count);
            Assert.Equal(2, result.Value.Types.Count);
            Assert.Equal("Angular", result.Value.Brands[0]);
        }
    }
}
