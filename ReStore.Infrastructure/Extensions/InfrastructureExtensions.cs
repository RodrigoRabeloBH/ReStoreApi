using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReStore.Domain.Entities;
using ReStore.Domain.Interfaces;
using ReStore.Infrastructure.Data;
using System.Diagnostics.CodeAnalysis;

namespace ReStore.Infrastructure.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddInfrastructureExtensions(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<StoreContext>(opt =>
            {
                opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddTransient<IStoreRepository, StoreRepository>();

            services.AddIdentityCore<User>(opt => { opt.User.RequireUniqueEmail = true; })
                .AddRoles<Role>()
                .AddEntityFrameworkStores<StoreContext>();

            return services;
        }
    }
}
