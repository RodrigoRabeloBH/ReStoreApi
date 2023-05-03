using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ReStore.Application.Models
{    
    [ExcludeFromCodeCoverage]
    public class BasketModel
    {
        public int Id { get; set; }
        public string BuyerId { get; set; }
        public List<BasketItemModel> Items { get; set; }
        public string PaymentIntentId { get; set; }
        public string ClientSecret { get; set; }
    }
}
