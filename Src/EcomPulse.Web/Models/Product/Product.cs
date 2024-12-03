using System;
using System.ComponentModel.DataAnnotations;

namespace EcomPulse.Web.Models;
    public class Product
    {
        public Product()
        {
            Description = String.Empty;
        }
        [Key]
        public Guid Id { get; set; }
        [MaxLength(100)]
        public required string Name { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
        public required decimal Price { get; set; }
        public  string? ImageUrl { get; set; }
        public  Guid CategoryId { get; set; }

        public virtual Category Category { get; set; }
    }