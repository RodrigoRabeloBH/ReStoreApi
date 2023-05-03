using ReStore.Domain.Entities;
using System.Threading.Tasks;

namespace ReStore.Application.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateToken(User user);
    }
}
