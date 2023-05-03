using System.Diagnostics.CodeAnalysis;

namespace ReStore.Application.Models
{
    [ExcludeFromCodeCoverage]
    public class UserModel
    {
        public string Email { get; set; }

        public string UserName { get; set; }

        public string Token { get; set; }

        public BasketModel Basket { get; set; }
    }
}
