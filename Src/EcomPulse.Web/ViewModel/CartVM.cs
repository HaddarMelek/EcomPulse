using EcomPulse.Web.Models;

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

    public class CartItemVM
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal ProductPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => ProductPrice * Quantity;
    }
}