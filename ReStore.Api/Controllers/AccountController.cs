using API.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReStore.Api.Extensions;
using ReStore.Application.Interfaces;
using ReStore.Application.Models;
using ReStore.Domain.Entities;
using ReStore.Infrastructure.Data;
using System.Threading.Tasks;

namespace ReStore.Api.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly IAccountServices _accountServices;

        private readonly StoreContext _context;

        public AccountController(IAccountServices accountServices, StoreContext context)
        {
            _accountServices = accountServices;
            _context = context;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserModel>> Login(UserLoginModel userLoginModel)
        {
            var buyerId = Request?.Cookies["buyerId"];

            var user = await _accountServices.Login(userLoginModel);

            if (user == null) return Unauthorized();

            var userBasket = await RetrieveBasket(userLoginModel.UserName);
            var anonBasket = await RetrieveBasket(buyerId);

            if (anonBasket != null)
            {
                if (userBasket != null)
                    _context.Baskets.Remove(userBasket);

                anonBasket.BuyerId = user.UserName;
                Response.Cookies.Delete("buyerId");
                await _context.SaveChangesAsync();
            }

            user.Basket = anonBasket.MapBasketToBasketModel() ?? userBasket.MapBasketToBasketModel();

            return user;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(UserRegisterModel userRegisterModel)
        {
            var result = await _accountServices.Register(userRegisterModel);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return ValidationProblem();
            }

            return StatusCode(201);
        }

        [Authorize]
        [HttpGet("currentUser")]
        public async Task<ActionResult<UserModel>> GetCurrentUser()
        {
            string name = User?.Identity?.Name;

            var user = await _accountServices.GetCurrentUser(name);

            return user;
        }  

        private async Task<Basket> RetrieveBasket(string buyerId)
        {
            if (string.IsNullOrWhiteSpace(buyerId))
            {
                Response.Cookies.Delete("buyerId");

                return null;
            }

            return await _context.Baskets
                .Include(i => i.Items)
                .ThenInclude(p => p.Product)
                .FirstOrDefaultAsync(x => x.BuyerId == buyerId);
        }
    }
}
