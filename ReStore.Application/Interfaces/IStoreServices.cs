using ReStore.Application.Models;
using ReStore.Application.RequestHelpers;
using ReStore.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReStore.Applicationfaces
{
    public interface IStoreServices
    {
        Task<PagedList<Product>> GetProducts(ProductParams productParams);
        Task<Product> GetProductById(int id);
        Task<BasketModel> GetBasketByBuyerId(string buyerId);
        Task CreateBasket(BasketModel basketModel);
        Task<BasketModel> AddItemToBasket(int productId, int quantity, string buyerId);
        Task<bool> RemoveItemFromBasket(int productId, int quantity, string buyerId);
        Task<ProductFilter> GetBrandsAndTypes();
        Task<List<OrderModel>> GetOrders(string buyerId);
        Task<OrderModel> GetOrder(int id, string buyerId);
        void RemoveBasket(BasketModel basketModel);
        Task<Order> CreateOrder(CreateOrderModel orderModel, string buyerId);
        Task<UserAddress> GetUserAddress(string username);
        Task<BasketModel> CreateOrUpdatePaymentIntent(string buyerId);
    }
}
