using System.Diagnostics.CodeAnalysis;

namespace ReStore.Application.RequestHelpers
{
    [ExcludeFromCodeCoverage]
    public class MetaData
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }
}
