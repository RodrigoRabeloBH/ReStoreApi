using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace ReStore.Domain.Entities.OrderAggregate
{
    [ExcludeFromCodeCoverage]
    [Owned]
    public class ProductItemOrdered
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string PictureUrl { get; set; }
    }
}
