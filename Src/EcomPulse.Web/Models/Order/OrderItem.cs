using System;
using System.ComponentModel.DataAnnotations;

namespace EcomPulse.Web.Models;

public class OrderItem
{
    [Key] public Guid Id { get; set; }

    public Guid OrderId { get; set; }
    public virtual Order Order { get; set; }

    public Guid ProductId { get; set; }
    public virtual Product Product { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
    public int Quantity { get; set; }

    [Range(0, double.MaxValue)] public decimal Price { get; set; }
}