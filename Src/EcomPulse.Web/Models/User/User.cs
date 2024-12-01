using System.ComponentModel.DataAnnotations;

namespace EcomPulse.Web.Models
{
    public class User
    {
        public User()
        {
            Orders = new List<Order>();
            Carts = new List<Cart>();
        }

        [Key]
        public Guid Id { get; set; }

        [MaxLength(100)]
        public required string UserName { get; set; }

        [EmailAddress]
        [MaxLength(200)]
        public required string Email { get; set; }

        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        public required string Password { get; set; }

        public required ICollection<Order> Orders { get; set; }
        public required ICollection<Cart> Carts { get; set; }
    }
}