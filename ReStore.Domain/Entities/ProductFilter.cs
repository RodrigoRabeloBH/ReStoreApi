using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ReStore.Domain.Entities
{
    [ExcludeFromCodeCoverage]
    public class ProductFilter
    {
        public List<string> Brands { get; set; }
        public List<string> Types { get; set; }
    }
}
