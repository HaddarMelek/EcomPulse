using EcomPulse.Web.Models;

namespace EcomPulse.Web.ViewModel
{
    public class CartVM
    {
        
        public CartVM()
        {
            UserName= string.Empty;
            CartItems = new List<CartItemVM>();
            
        }
        public Guid CartId { get; set; }
        public Guid UserId { get; set; }
        public string? UserName { get; set; }  
        public List<CartItemVM> CartItems { get; set; } 
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