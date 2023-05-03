using System.Diagnostics.CodeAnalysis;

namespace ReStore.Application.RequestHelpers
{
    [ExcludeFromCodeCoverage]
    public class ProductParams : PaginationParams
    {
        public string OrderBy { get; set; }
        public string SearchTerm { get; set; }
        public string Types { get; set; }
        public string Brands { get; set; }
    }
}
