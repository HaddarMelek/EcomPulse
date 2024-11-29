namespace EcomPulse.Web.ViewModel;

public class OrderSummaryVM
{
    public Guid OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = "Pending";
}