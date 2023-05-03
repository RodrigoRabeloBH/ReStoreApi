using ReStore.Domain.Entities.OrderAggregate;
using System.Collections.Generic;
using System;
using System.Diagnostics.CodeAnalysis;

namespace ReStore.Application.Models
{
    [ExcludeFromCodeCoverage]
    public class OrderModel
    {
        public int Id { get; set; }
        public string BuyerId { get; set; }
        public ShippingAddress ShippingAddress { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public List<OrderItemModel> OrderItems { get; set; }
        public long Subtotal { get; set; }
        public long DeliveryFee { get; set; }
        public string OrderStatus { get; set; }
        public long Total { get; set; }
    }
}
