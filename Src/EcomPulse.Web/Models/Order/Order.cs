using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace EcomPulse.Web.Models;

public class Order
{
    public Order()
    {
        OrderItems = new List<OrderItem>();
    }

    [Key] public Guid Id { get; set; }

    public virtual IdentityUser User { get; set; }

    [Range(0, double.MaxValue)] public decimal Total { get; set; }

    public DateTime OrderDate { get; set; }

    [Required] [MaxLength(500)] public required string ShippingAddress { get; set; }

    public string Status { get; set; } = "Pending";

    public virtual ICollection<OrderItem> OrderItems { get; set; }
}