using Microsoft.EntityFrameworkCore;
using EcomPulse.Web.Models;

namespace EcomPulse.Web.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<EcomPulse.Web.Models.Cart> Cart { get; set; } = default!;
        public DbSet<EcomPulse.Web.Models.Order> Order { get; set; } = default!;
        public DbSet<EcomPulse.Web.Models.User> User { get; set; } = default!;
        //public DbSet<Cart> Carts { get; set; }
        //public DbSet<CartItem> CartItems { get; set; }
        //public DbSet<Order> Orders { get; set; }
        //public DbSet<OrderItem> OrderItems { get; set; }
        //public DbSet<User> Users { get; set; }
    }
}