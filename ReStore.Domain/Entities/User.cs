using Microsoft.AspNetCore.Identity;
using System.Diagnostics.CodeAnalysis;

namespace ReStore.Domain.Entities
{
    [ExcludeFromCodeCoverage]
    public class User : IdentityUser<int>
    {
        public UserAddress Address { get; set; }
    }
}
