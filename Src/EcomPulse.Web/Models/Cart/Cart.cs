using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace EcomPulse.Web.Models;

public class Cart
{
    public Cart()
    {
        CartItems = new List<CartItem>();
    }

    [Key] public Guid Id { get; set; }

    public virtual IdentityUser User { get; set; }
    public virtual ICollection<CartItem> CartItems { get; set; }
}