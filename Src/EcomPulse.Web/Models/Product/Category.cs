using System.ComponentModel.DataAnnotations;

namespace EcomPulse.Web.Models;
    public class Category
    {
        public Category()
        {
            Products = new List<Product>();
        }
        [Key]
        public Guid Id { get; set; }
        public required string Name { get; set; }
        
        public virtual ICollection<Product> Products { get; set; }
    }