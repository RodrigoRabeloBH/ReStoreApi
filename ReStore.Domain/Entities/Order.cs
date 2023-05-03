using ReStore.Domain.Entities.OrderAggregate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ReStore.Domain.Entities
{
    [ExcludeFromCodeCoverage]
    public class Order : BaseEntity
    {
        public string BuyerId { get; set; }

        [Required]
        public ShippingAddress ShippingAddress { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public List<OrderItem> OrderItems { get; set; }
        public long Subtotal { get; set; }
        public long DeliveryFee { get; set; }
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
        public string PaymentIntentId { get; set; }

        public long GetTotal()
        {
            return Subtotal + DeliveryFee;
        }
    }
}
