using EcomPulse.Web.Services;
using EcomPulse.Web.ViewModel.Product;
using Microsoft.AspNetCore.Mvc;

namespace EcomPulse.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(ILogger<ProductController> logger, ProductService productService)
        {
            _productService = productService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            var categories =await _productService.GetAllCategoriesAsync(); 
            var productVMs = products.Select(product => new ProductVM
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name,

                Categories = categories.Select(category => new CategoryVM
                {
                    Id = category.Id,
                    Name = category.Name
                }).ToList()
                
            }).ToList();

            return View(productVMs);
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productService.GetProductByIdAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            var productVm = new ProductVM
            {
                Id = id.Value,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name,
                Categories = new List<CategoryVM>() 
            };

            return View(productVm);
        }

        public async Task<IActionResult> Create()
        {
            var pvm = new ProductVM
            {
                
                Categories = (await _productService.GetAllCategoriesAsync())
                    .Select(c => new CategoryVM
                    {
                        Id = c.Id,
                        Name = c.Name
                    })
                    .ToList()
            };

            return View(pvm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductVM productVm)
        {
            if (ModelState.IsValid)
            {
                var result = await _productService.CreateProductAsync(
                    productVm.Name,
                    productVm.Description,
                    productVm.Price,
                    productVm.ImageUrl,
                    productVm.CategoryId
                );

                return result ? RedirectToAction(nameof(Index)) : BadRequest(productVm);
            }

            productVm.Categories = (await _productService.GetAllCategoriesAsync())
                .Select(c => new CategoryVM
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToList();

            return View(productVm);
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productService.GetProductByIdAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            var productVm = new ProductVM
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name,
                Categories = (await _productService.GetAllCategoriesAsync())
                    .Select(c => new CategoryVM
                    {
                        Id = c.Id,
                        Name = c.Name
                    })
                    .ToList()
            };

            return View(productVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ProductVM productVm)
        {
            if (id != productVm.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _productService.UpdateProductAsync(
                    id,
                    productVm.Name,
                    productVm.Description,
                    productVm.Price,
                    productVm.ImageUrl,
                    productVm.CategoryId,
                    productVm.CategoryName
                    );
                return RedirectToAction(nameof(Index));
            }

            productVm.Categories = (await _productService.GetAllCategoriesAsync())
                .Select(c => new CategoryVM
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToList();

            return View(productVm);
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productService.GetProductByIdAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            var productVm = new ProductVM
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name,

                Categories = (await _productService.GetAllCategoriesAsync())
                    .Select(c => new CategoryVM
                    {
                        Id = c.Id,
                        Name = c.Name
                    })
                    .ToList()
                
            };

            return View(productVm);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _productService.DeleteProductAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
