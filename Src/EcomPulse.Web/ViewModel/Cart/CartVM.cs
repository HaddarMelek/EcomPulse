
namespace EcomPulse.Web.ViewModel
{
    public class CartVM
    {
        public Guid CartId { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public List<CartItemVM> CartItems { get; set; } = new List<CartItemVM>();
        public decimal TotalPrice => CartItems.Sum(item => item.TotalPrice);
    }

    
}
