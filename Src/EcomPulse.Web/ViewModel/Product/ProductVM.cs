using System;
using System.Collections.Generic;

namespace EcomPulse.Web.ViewModel.Product
{
    public class ProductVM
    {
        public ProductVM()
        {
            Name = string.Empty;
            Description = string.Empty;
            Price = 0;
            CategoryName = string.Empty;

            Categories = new List<CategoryVM>();
        }
        public Guid Id { get; set; } 

        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public IList<CategoryVM> Categories { get; set; }
    }
}