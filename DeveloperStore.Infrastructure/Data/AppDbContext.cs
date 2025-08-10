using DeveloperStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeveloperStore.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<Cart> Carts => Set<Cart>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Sale> Sales => Set<Sale>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configura a tabela Product
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Title).IsRequired().HasMaxLength(200);
                entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Sale>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Number).IsRequired().HasMaxLength(20);
                entity.Property(s => s.Customer).IsRequired().HasMaxLength(100);
                entity.Property(s => s.Branch).HasMaxLength(100);
                entity.HasMany(s => s.Items)
                      .WithOne()
                      .HasForeignKey("SaleId") // FK virtual
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<SaleItem>(entity =>
            {
                entity.HasKey(i => i.Id);
                entity.Property(i => i.UnitPrice).HasColumnType("decimal(18,2)");
                entity.Property(i => i.Total).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.OwnsOne(u => u.Name);
                entity.OwnsOne(u => u.Address, address =>
                {
                    address.OwnsOne(a => a.Geolocation);
                });
            });

            modelBuilder.Entity<Cart>(entity =>
            {
                entity.OwnsMany(c => c.Products, b =>
                {
                    b.WithOwner();
                    b.Property(ci => ci.ProductId).IsRequired();
                    b.Property(ci => ci.Quantity).IsRequired();
                });
            });


        }
    }
}
