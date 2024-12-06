using System;
using System.ComponentModel.DataAnnotations;

namespace EcomPulse.Web.Models;

public class CartItem
{
    [Key] public Guid Id { get; set; }

    public Guid CartId { get; set; }
    public virtual Cart Cart { get; set; }

    public Guid ProductId { get; set; }
    public virtual Product Product { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
    public int Quantity { get; set; }
}