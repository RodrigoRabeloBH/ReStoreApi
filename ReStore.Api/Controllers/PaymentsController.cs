using API.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReStore.Application.Models;
using ReStore.Applicationfaces;
using System.Threading.Tasks;

namespace ReStore.Api.Controllers
{
    [Authorize]
    public class PaymentsController : BaseApiController
    {
        private readonly IStoreServices _storeServices;

        public PaymentsController(IStoreServices storeServices)
        {
            _storeServices = storeServices;
        }

        [HttpPost]
        public async Task<ActionResult<BasketModel>> CreateOrUpdatePaymentIntent()
        {
            var buyerId = User?.Identity?.Name;

            var basketModel = await _storeServices.CreateOrUpdatePaymentIntent(buyerId);

            if (basketModel == null)
                return BadRequest(new ProblemDetails { Title = "Problem creating payment intent" });

            return basketModel;
        }
    }
}
