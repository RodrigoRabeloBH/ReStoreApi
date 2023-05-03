using System.Diagnostics.CodeAnalysis;

namespace ReStore.Application.Models
{
    [ExcludeFromCodeCoverage]
    public class OrderItemModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string PictureUrl { get; set; }
        public long Price { get; set; }
        public int Quantity { get; set; }
    }
}
