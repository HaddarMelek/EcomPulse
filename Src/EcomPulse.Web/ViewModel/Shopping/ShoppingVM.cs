using EcomPulse.Web.ViewModel.Product;

namespace EcomPulse.Web.ViewModel.Shopping;

public class ShoppingVM
{
    public List<CategoryVM> Categories { get; set; }
    public List<ProductVM> Products { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public Guid? SelectedCategoryId { get; set; }
}
