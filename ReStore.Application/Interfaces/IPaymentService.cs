using ReStore.Domain.Entities;
using Stripe;
using System.Threading.Tasks;

namespace ReStore.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentIntent> CreateOrUpdatePaymentIntent(Basket basket);
    }
}
