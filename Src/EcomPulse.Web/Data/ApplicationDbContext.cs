using Microsoft.EntityFrameworkCore;
using EcomPulse.Web.Models;

namespace EcomPulse.Web.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; } 
        public DbSet<User> Users { get; set; } 
    }
}