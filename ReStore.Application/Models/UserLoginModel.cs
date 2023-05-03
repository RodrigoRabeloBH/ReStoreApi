using System.Diagnostics.CodeAnalysis;

namespace ReStore.Application.Models
{  
    [ExcludeFromCodeCoverage]
    public class UserLoginModel
    {
        public string UserName { get; set; }

        public string Password { get; set; }
    }
}
