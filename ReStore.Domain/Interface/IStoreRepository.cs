using ReStore.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReStore.Domain.Interfaces
{
    public interface IStoreRepository
    {
        IQueryable<Product> GetAllProducts(string orderBy, string searchTerm, string brands, string types);
        Task<Product> GetProductById(int id);
        Task<Basket> RetrieveBasket(string buyerId);
        Task CreateBasket(Basket basket);
        Task<bool> SaveChanges();
        Task<bool> RemoveItemFromBasket(int productId, int quantity, string buyerId);
        Task<ProductFilter> GetBrandsAndTypes();
        void RemoveBasket(Basket basket);
        Task<List<Order>> GetOrders(string buyerId);
        Task<Order> GetOrder(int id, string buyerId);
        Order CreateOrder(Order order);
        Task<User> GetUser(string username);
        void UpdateUser(User user);
        Task<UserAddress> GetUserAddress(string username);
        Task<bool> UpdateBasket(Basket basket);
    }
}
