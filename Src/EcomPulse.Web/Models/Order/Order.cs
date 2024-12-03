using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EcomPulse.Web.Models
{
    public class Order
    {

        public Order()
        {
            OrderItems = new List<OrderItem>();
        }

        [Key]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public required User User { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Total { get; set; }

        public DateTime OrderDate { get; set; }

        [Required]
        [MaxLength(500)]
        public required string ShippingAddress { get; set; }
        
        public string Status { get; set; } = "Pending";

        public required ICollection<OrderItem> OrderItems { get; set; }
    }
}