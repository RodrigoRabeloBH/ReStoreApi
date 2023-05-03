using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ReStore.Domain.Entities;
using ReStore.Domain.Interfaces;
using ReStore.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace ReStore.Infrastructure.Data
{
    [ExcludeFromCodeCoverage]
    public class StoreRepository : IStoreRepository
    {
        private readonly StoreContext _context;

        private readonly ILogger<StoreRepository> _logger;

        public StoreRepository(StoreContext context, ILogger<StoreRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> SaveChanges()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task CreateBasket(Basket basket)
        {
            try
            {
                _context.Baskets.Add(basket);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        public IQueryable<Product> GetAllProducts(string orderBy, string searchTerm, string brands, string types)
        {
            IQueryable<Product> products = null;

            try
            {
                products = _context.Products
                    .Sort(orderBy)
                    .Search(searchTerm)
                    .Filter(brands, types)
                    .AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            return products;
        }

        public async Task<Product> GetProductById(int id)
        {
            Product product = null;

            try
            {
                product = await _context.Products.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            return product;
        }

        public async Task<bool> UpdateBasket(Basket basket)
        {
            bool result = false;

            try
            {
                _context.Baskets.Update(basket);

                result = await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            return result;
        }

        public async Task<bool> RemoveItemFromBasket(int productId, int quantity, string buyerId)
        {
            bool result = false;

            try
            {
                var basket = await RetrieveBasket(buyerId);

                if (basket == null) return result;

                basket.RemoveItem(productId, quantity);

                result = await _context.SaveChangesAsync() > 0;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            return result;
        }

        public async Task<Basket> RetrieveBasket(string buyerId)
        {
            Basket basket = null;

            try
            {
                basket = await _context.Baskets
                    .Include(i => i.Items)
                    .ThenInclude(p => p.Product)
                    .FirstOrDefaultAsync(b => b.BuyerId == buyerId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            return basket;
        }

        public async Task<ProductFilter> GetBrandsAndTypes()
        {
            var brands = await _context.Products.Select(p => p.Brand).Distinct().ToListAsync();
            var types = await _context.Products.Select(p => p.Type).Distinct().ToListAsync();

            return new ProductFilter { Brands = brands, Types = types };
        }

        public void RemoveBasket(Basket basket)
        {
            try
            {
                _context.Baskets.Remove(basket);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        public async Task<List<Order>> GetOrders(string buyerId)
        {
            List<Order> orders = null;

            try
            {
                orders = await _context.Orders
                    .Include(o => o.OrderItems)
                    .Where(x => x.BuyerId == buyerId).ToListAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            return orders;
        }

        public async Task<Order> GetOrder(int id, string buyerId)
        {
            Order order = null;

            try
            {
                order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstAsync(o => o.Id == id && o.BuyerId == buyerId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            return order;
        }

        public Order CreateOrder(Order order)
        {
            try
            {
                _context.Orders.Add(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            return order;
        }

        public async Task<User> GetUser(string username)
        {
            User user = null;

            try
            {
                user = await _context.Users
                    .Include(u => u.Address)
                    .FirstOrDefaultAsync(x => x.UserName == username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            return user;
        }

        public async Task<UserAddress> GetUserAddress(string username)
        {
            UserAddress address = null;

            try
            {
                address = await _context.Users
                    .Where(u => u.UserName == username)
                    .Select(user => user.Address)
                    .FirstOrDefaultAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            return address;
        }

        public void UpdateUser(User user)
        {
            try
            {
                _context.Users.Attach(user).State = EntityState.Modified;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
    }
}
