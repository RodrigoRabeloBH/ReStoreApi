using AutoMapper;
using Microsoft.Extensions.Logging;
using ReStore.Application.Interfaces;
using ReStore.Application.Models;
using ReStore.Application.RequestHelpers;
using ReStore.Applicationfaces;
using ReStore.Domain.Entities;
using ReStore.Domain.Entities.OrderAggregate;
using ReStore.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReStore.Application.Implementations
{
    public class StoreServices : IStoreServices
    {
        private readonly IStoreRepository _repository;

        private readonly ILogger<StoreServices> _logger;

        private readonly IMapper _mapper;

        private readonly IPaymentService _paymentService;

        private readonly IMailService _mailService;

        public StoreServices(IStoreRepository repository, ILogger<StoreServices> logger,
            IMapper mapper, IPaymentService paymentService, IMailService mailService)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _paymentService = paymentService;
            _mailService = mailService;
        }

        public async Task<PagedList<Product>> GetProducts(ProductParams productParams)
        {
            _logger.LogInformation("[Getting all products]");

            var query = _repository.GetAllProducts(productParams.OrderBy, productParams.SearchTerm, productParams.Brands, productParams.Types);

            var products = await PagedList<Product>.ToPagedList(query, productParams.PageNumber, productParams.PageSize);

            return products;
        }

        public async Task<Product> GetProductById(int id)
        {
            _logger.LogInformation("[Getting product by id]");

            var product = await _repository.GetProductById(id);

            if (product != null)
                _logger.LogInformation($"Product: {product.Name}");

            return product;
        }

        public async Task<BasketModel> GetBasketByBuyerId(string buyerId)
        {
            _logger.LogInformation("[Getting basket for buyerid: {0}]", buyerId);

            var basket = await _repository.RetrieveBasket(buyerId);

            var basketToReturn = _mapper.Map<BasketModel>(basket);

            return basketToReturn;
        }

        public async Task CreateBasket(BasketModel basketModel)
        {
            _logger.LogInformation("[Creating basket ...]");

            var basket = _mapper.Map<Basket>(basketModel);

            await _repository.CreateBasket(basket);
        }

        public async Task<BasketModel> AddItemToBasket(int productId, int quantity, string buyerId)
        {
            _logger.LogInformation("[Adding item to basket ...]");

            Basket basket = null;

            try
            {
                basket = await _repository.RetrieveBasket(buyerId);

                if (basket == null)
                {
                    basket = new Basket { BuyerId = buyerId };

                    await _repository.CreateBasket(basket);
                };

                var product = await _repository.GetProductById(productId);

                if (product == null) return null;

                basket.AddItem(product, quantity);

                await _repository.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            return _mapper.Map<BasketModel>(basket);
        }

        public async Task<bool> RemoveItemFromBasket(int productId, int quantity, string buyerId)
        {
            _logger.LogInformation("[Removing item from basket ...]");

            var result = await _repository.RemoveItemFromBasket(productId, quantity, buyerId);

            return result;
        }

        public async Task<ProductFilter> GetBrandsAndTypes()
        {
            var bransdAndTypes = await _repository.GetBrandsAndTypes();

            return bransdAndTypes;
        }

        public async Task<List<OrderModel>> GetOrders(string buyerId)
        {
            var orders = await _repository.GetOrders(buyerId);

            var ordersModel = _mapper.Map<List<OrderModel>>(orders);

            return ordersModel;
        }

        public async Task<OrderModel> GetOrder(int id, string buyerId)
        {
            var order = await _repository.GetOrder(id, buyerId);

            var orderModel = _mapper.Map<OrderModel>(order);

            return orderModel;
        }

        public void RemoveBasket(BasketModel basketModel)
        {
            var basket = _mapper.Map<Basket>(basketModel);

            _repository.RemoveBasket(basket);
        }

        public async Task<Order> CreateOrder(CreateOrderModel orderModel, string buyerId)
        {
            Order order = null;

            try
            {
                var basket = await _repository.RetrieveBasket(buyerId);

                if (basket == null)
                {
                    _logger.LogError("Could not locate basket");

                    return order;
                }

                var items = await GetOrderItems(basket);

                var subtotal = items.Sum(item => item.Price * item.Quantity);

                var deliverFee = subtotal > 1000 ? 0 : 50;

                order = new Order
                {
                    OrderItems = items,
                    BuyerId = buyerId,
                    ShippingAddress = orderModel.ShippingAddress,
                    Subtotal = subtotal,
                    DeliveryFee = deliverFee,
                    PaymentIntentId = basket.PaymentIntentId
                };

                order = _repository.CreateOrder(order);

                var user = await _repository.GetUser(buyerId);

                _repository.RemoveBasket(basket);

                if (orderModel.SaveAddress)
                {
                    var address = new UserAddress
                    {
                        FullName = orderModel.ShippingAddress.FullName,
                        Street = orderModel.ShippingAddress.Street,
                        Neighborhood = orderModel.ShippingAddress.Neighborhood,
                        Number = orderModel.ShippingAddress.Number,
                        Complement = order.ShippingAddress.Complement,
                        City = orderModel.ShippingAddress.City,
                        State = orderModel.ShippingAddress.State,
                        Country = orderModel.ShippingAddress.Country,
                        Zipcode = orderModel.ShippingAddress.Zipcode,
                    };

                    user.Address = address;
                }

                var success = await _repository.SaveChanges();

                if (success)
                    await _mailService.SendEmailAsync(order, user);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            return order;
        }

        public async Task<UserAddress> GetUserAddress(string username)
        {
            return await _repository.GetUserAddress(username);
        }

        public async Task<BasketModel> CreateOrUpdatePaymentIntent(string buyerId)
        {
            var basket = await _repository.RetrieveBasket(buyerId);

            if (basket == null) return null;

            var paymentIntent = await _paymentService.CreateOrUpdatePaymentIntent(basket);

            if (paymentIntent == null) return null;

            basket.PaymentIntentId = basket.PaymentIntentId ?? paymentIntent.Id;

            basket.ClientSecret = basket.ClientSecret ?? paymentIntent.ClientSecret;

            var result = await _repository.UpdateBasket(basket);

            if (!result) return null;

            var basketModel = _mapper.Map<BasketModel>(basket);

            return basketModel;
        }

        private async Task<List<OrderItem>> GetOrderItems(Basket basket)
        {
            var items = new List<OrderItem>();

            foreach (var item in basket.Items)
            {
                var productItem = await _repository.GetProductById(item.ProductId);

                var itemOrdered = new ProductItemOrdered
                {
                    ProductId = productItem.Id,
                    Name = productItem.Name,
                    PictureUrl = productItem.PictureUrl
                };

                var orderItem = new OrderItem
                {
                    ItemOrdered = itemOrdered,
                    Price = productItem.Price,
                    Quantity = item.Quantity
                };

                items.Add(orderItem);

                productItem.QuantityInStock -= item.Quantity;
            }

            return items;
        }
    }
}
