using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReStore.Application.Models;
using ReStore.Applicationfaces;

namespace API.Controllers
{
    public class BasketController : BaseApiController
    {
        private readonly IStoreServices _services;

        public BasketController(IStoreServices services)
        {
            _services = services;
        }

        [HttpGet(Name = "GetBasket")]
        public async Task<ActionResult<BasketModel>> GetBasket()
        {
            string buyerId = Request?.Cookies["buyerId"];

            var basket = await _services.GetBasketByBuyerId(buyerId);

            if (basket == null) return NotFound();

            return basket;
        }

        [HttpPost]
        public async Task<ActionResult<BasketModel>> AddItemToBasket(int productId, int quantity)
        {
            string buyerId = GetBuyerId();

            var basketModel = await _services.GetBasketByBuyerId(buyerId);

            if (basketModel == null) buyerId = CreateCookie();

            basketModel = await _services.AddItemToBasket(productId, quantity, buyerId);

            if (basketModel != null) return CreatedAtRoute("GetBasket", basketModel);

            return BadRequest(new ProblemDetails { Title = "Problem saving to basket" });
        }

        [HttpDelete]
        public async Task<ActionResult> RemoveItemFromBasket(int productId, int quantity)
        {
            string buyerId = GetBuyerId();

            var result = await _services.RemoveItemFromBasket(productId, quantity, buyerId);

            if (result) return Ok();

            return BadRequest(new ProblemDetails { Title = "Problem removing item from the basket" });
        }

        [HttpDelete("removeCookie")]
        public void RemoveCookie()
        {
            Response.Cookies.Delete("buyerId");
        }
        private string CreateCookie()
        {
            var buyerId = User?.Identity?.Name ?? Guid.NewGuid().ToString();

            var cookieOptions = new CookieOptions { IsEssential = true, Expires = DateTime.Now.AddDays(30) };

            Response?.Cookies?.Append("buyerId", buyerId, cookieOptions);

            return buyerId;
        }

        private string GetBuyerId()
        {
            return User?.Identity?.Name ?? Request?.Cookies["buyerId"];
        }
    }
}