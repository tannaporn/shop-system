using Microsoft.EntityFrameworkCore;
using shop_api_layer.Models.Cart;
using shop_api_layer.Models.Customer;
using shop_api_layer.Models.Order;
using shop_api_layer.Models.Product;
using shop_api_layer.Models.UserRole;
using OrderModel = shop_api_layer.Models.Order.Order;

namespace shop_api_layer.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // existing
        public DbSet<Product> Products => Set<Product>();
       

        // new
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Cart> Carts => Set<Cart>();
        public DbSet<CartItem> CartItems => Set<CartItem>();
        public DbSet<OrderModel> Orders => Set<OrderModel>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           

            // CartItem: unique (cart_id, product_id)
            modelBuilder.Entity<CartItem>()
                .HasIndex(ci => new { ci.CartId, ci.ProductId })
                .IsUnique();

            // explicit relationships — prevents EF convention scan from picking up unwanted types
            modelBuilder.Entity<OrderModel>()
                .HasMany(o => o.Items)
                .WithOne(i => i.Order)
                .HasForeignKey(i => i.OrderId);

            modelBuilder.Entity<Cart>()
                .HasMany(c => c.Items)
                .WithOne()
                .HasForeignKey(i => i.CartId);
        }
    }
}
