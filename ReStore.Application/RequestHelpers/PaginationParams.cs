using System.Diagnostics.CodeAnalysis;

namespace ReStore.Application.RequestHelpers
{
    public class PaginationParams
    {
        private const int MaxResultsPerPage = 36;

        private int _pageSize = 6;
        public int PageNumber { get; set; } = 1;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxResultsPerPage ? MaxResultsPerPage : value;
        }
    }
}
