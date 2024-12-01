
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

    
}
