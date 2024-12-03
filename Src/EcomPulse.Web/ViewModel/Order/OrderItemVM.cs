using System;

namespace EcomPulse.Web.ViewModel;

public class OrderItemVM
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}