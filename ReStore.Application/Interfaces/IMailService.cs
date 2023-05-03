using ReStore.Domain.Entities;
using System.Threading.Tasks;

namespace ReStore.Application.Interfaces
{
    public interface IMailService
    {
        Task SendEmailAsync(Order order, User user);
    }
}
