using EcomPulse.Web.Data;
using EcomPulse.Web.Models;

namespace EcomPulse.Web.ViewModel;

public class ProductVM
{
    public ProductVM()
    {
        Product = null!;
        Categories = new List<Category>();
    }
    public Product Product { get; set; }
    public IList<Category> Categories { get; set; }
}