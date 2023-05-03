using System.Diagnostics.CodeAnalysis;

namespace ReStore.Application.Models
{
    [ExcludeFromCodeCoverage]
    public class UserRegisterModel : UserLoginModel
    {
        public string Email { get; set; }
    }
}
