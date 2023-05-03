using System.Diagnostics.CodeAnalysis;

namespace ReStore.Domain.Entities
{
    [ExcludeFromCodeCoverage]
    public class UserAddress : Address
    {
        public int Id { get; set; }
    }
}
