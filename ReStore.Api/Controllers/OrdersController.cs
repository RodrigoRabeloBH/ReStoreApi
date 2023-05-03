using API.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReStore.Application.Interfaces;
using ReStore.Application.Models;
using ReStore.Applicationfaces;
using ReStore.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReStore.Api.Controllers
{
    [Authorize]
    public class OrdersController : BaseApiController
    {
        private readonly IStoreServices _services;

        private readonly IMailService _mailService;

        public OrdersController(IStoreServices services, IMailService mailService)
        {
            _services = services;
            _mailService = mailService;
        }

        [HttpGet]
        public async Task<ActionResult<List<OrderModel>>> GetOrders()
        {
            string buyerId = User.Identity.Name;

            var orders = await _services.GetOrders(buyerId);

            return orders;
        }

        [HttpGet("{id}", Name = "GetOrder")]
        public async Task<ActionResult<OrderModel>> GetOrder(int id)
        {
            string buyerId = User.Identity.Name;

            var order = await _services.GetOrder(id, buyerId);

            return order;
        }

        [HttpPost]
        public async Task<ActionResult<int>> CreateOrder(CreateOrderModel orderModel)
        {
            var resutl = await _services.CreateOrder(orderModel, User.Identity.Name);

            if (resutl != null)
                return CreatedAtRoute("GetOrder", new { id = resutl.Id }, resutl.Id);

            return BadRequest("Problem creating order");
        }

        [HttpGet("savedAddress")]
        public async Task<ActionResult<UserAddress>> GetSavedAddress()
        {
            var username = User?.Identity?.Name;

            var address = await _services.GetUserAddress(username);

            if (address == null)
                return NoContent();

            return address;
        }
    }
}
