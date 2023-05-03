using Microsoft.Extensions.DependencyInjection;
using ReStore.Application.Implementations;
using ReStore.Application.Interfaces;
using ReStore.Application.Mapping;
using ReStore.Applicationfaces;
using System.Diagnostics.CodeAnalysis;

namespace ReStore.Application.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddApplicationExtensions(this IServiceCollection services)
        {
            services.AddTransient<IStoreServices, StoreServices>();

            services.AddTransient<ITokenService, TokenService>();

            services.AddTransient<IAccountServices, AccountServices>();

            services.AddScoped<IPaymentService, PaymentService>();

            services.AddAutoMapper(typeof(MappingProfile));

            services.AddTransient<IMailService, MailService>();

            return services;
        }
    }
}
