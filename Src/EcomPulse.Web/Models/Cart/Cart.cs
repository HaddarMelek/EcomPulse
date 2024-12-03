using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EcomPulse.Web.Models
{
    public class Cart
    {
        public Cart()
        {
            CartItems = new List<CartItem>();
        }

        [Key]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public required User User { get; set; }

        public required ICollection<CartItem> CartItems { get; set; }
    }
}