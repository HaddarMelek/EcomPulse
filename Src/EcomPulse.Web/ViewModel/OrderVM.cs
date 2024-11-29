using EcomPulse.Web.Models;

namespace EcomPulse.Web.ViewModel

{
    public class OrderVM
    {
        public Order Order { get; set; }
        public IList<OrderItem> OrderItems { get; set; }
    }
}
