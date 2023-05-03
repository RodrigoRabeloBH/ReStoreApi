using ReStore.Domain.Entities.OrderAggregate;
using System.Diagnostics.CodeAnalysis;

namespace ReStore.Application.Models
{
    [ExcludeFromCodeCoverage]
    public class CreateOrderModel
    {
        public bool SaveAddress { get; set; }
        public ShippingAddress ShippingAddress { get; set; }
    }
}
