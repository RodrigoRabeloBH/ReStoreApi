using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReStore.Api.Extensions;
using ReStore.Application.RequestHelpers;
using ReStore.Applicationfaces;
using ReStore.Domain.Entities;

namespace API.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly IStoreServices _services;
        public ProductsController(IStoreServices services)
        {
            _services = services;
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<Product>>> GetProducts([FromQuery] ProductParams productParams)
        {
            var products = await _services.GetProducts(productParams);

            Response?.AddPaginationHeader(products?.MetaData);

            return products;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _services.GetProductById(id);

            if (product == null) return NotFound();

            return product;
        }

        [HttpGet("filters")]
        public async Task<ActionResult<ProductFilter>> GetFilters()
        {
            var productFilter = await _services.GetBrandsAndTypes();

            return productFilter;
        }
    }
}