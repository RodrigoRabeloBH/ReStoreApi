using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReStore.Domain.Entities;
using ReStore.Domain.Enum;
using System.Diagnostics.CodeAnalysis;

namespace ReStore.Infrastructure.Data
{
    [ExcludeFromCodeCoverage]
    public class StoreContext : IdentityDbContext<User, Role, int>
    {
        public DbSet<Product> Products { get; set; }

        public DbSet<Basket> Baskets { get; set; }

        public DbSet<Order> Orders { get; set; }

        public StoreContext() { }

        public StoreContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            var memberAccessLevel = AccessLevel.Member.ToString();
            var adminAccessLevel = AccessLevel.Admin.ToString();

            base.OnModelCreating(builder);

            builder.Entity<User>()
                .HasOne(a => a.Address)
                .WithOne()
                .HasForeignKey<UserAddress>(a => a.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Role>()
                .HasData
                (
                new Role { Id = 1, Name = memberAccessLevel, NormalizedName = memberAccessLevel.ToLower() },
                new Role { Id = 2, Name = adminAccessLevel, NormalizedName = adminAccessLevel.ToLower() }
                );
        }
    }
}
