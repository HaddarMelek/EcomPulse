namespace EcomPulse.Web.ViewModel;

public class OrderVM
{
    public Guid Id { get; set; }
    public decimal Total => OrderItems.Sum(oi => oi.TotalPrice);
    public DateTime OrderDate { get; set; } = DateTime.Now;
    public string ShippingAddress { get; set; }
    public string Status { get; set; } = "Pending";

    public List<OrderItemVM> OrderItems { get; set; }

    public OrderVM()
    {
        OrderItems = new List<OrderItemVM>();
    }
}