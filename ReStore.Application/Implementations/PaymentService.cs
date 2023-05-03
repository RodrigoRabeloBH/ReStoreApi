using Microsoft.Extensions.Configuration;
using ReStore.Application.Interfaces;
using ReStore.Domain.Entities;
using Stripe;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReStore.Application.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;

        public PaymentService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<PaymentIntent> CreateOrUpdatePaymentIntent(Basket basket)
        {
            StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"];

            var service = new PaymentIntentService();

            var intent = new PaymentIntent();

            var subtotal = basket.Items.Sum(i => i.Quantity * i.Product.Price);

            var deliveryFee = subtotal > 1000 ? 0 : 50;

            if (string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (subtotal + deliveryFee) * 100,
                    Currency = "brl",
                    PaymentMethodTypes = new List<string> { "card" }
                };

                intent = await service.CreateAsync(options);
            }
            else
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = (subtotal + deliveryFee) * 100
                };

                await service.UpdateAsync(basket.PaymentIntentId, options);
            }

            return intent;
        }
    }
}
