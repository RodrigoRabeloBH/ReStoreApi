using ReStore.Application.Models;
using ReStore.Domain.Entities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ReStore.Api.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class BasketExtensions
    {
        public static BasketModel MapBasketToBasketModel(this Basket basket)
        {
            if (basket == null) return null;

            return new BasketModel
            {
                Id = basket.Id,
                BuyerId = basket?.BuyerId,
                Items = basket.Items.Select(item => new BasketItemModel
                {
                    ProductId = item.ProductId,
                    Name = item.Product.Name,
                    Brand = item.Product.Brand,
                    PictureUrl = item.Product.PictureUrl,
                    Price = item.Product.Price,
                    Quantity = item.Quantity,
                    Type = item.Product.Type
                }).ToList()
            };
        }
    }
}
