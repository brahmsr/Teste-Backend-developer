using APItesteInside.Models.Domain;
using APItesteInside.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace APItesteInside.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }

        //criação do muitos para muitos dos produtos e ordens de compra
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //especificando as foreign keys
            modelBuilder.Entity<OrderProduct>()
                .HasKey(op => new { op.ProductId, op.OrderId });

            // um pedido para muitos produtos
            modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.Order)
                .WithMany(o => o.OrderProducts)
                .HasForeignKey(o => o.OrderId);

            // um produto para muitos pedidos
            modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.Product)
                .WithMany(o => o.OrderProducts)
                .HasForeignKey(o => o.ProductId);
        }
    }
}
