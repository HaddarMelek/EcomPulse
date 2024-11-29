using EcomPulse.Web.Models;

namespace EcomPulse.Web.ViewModel
{
    public class UserVM
    {
        public Guid UserId { get; set; }
        public List<OrderSummaryVM> Orders { get; set; } = new List<OrderSummaryVM>();
    }

   
}