using EcomPulse.Web.Services;
using EcomPulse.Web.ViewModel;
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

        // GET: Product
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            return View(products);
        }

        // GET: Product/Details/5
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

            return View(product);
        }

        // GET: Product/Create
        public async Task<IActionResult> Create()
        {
            var pvm = new ProductVM
            {
                Categories = await _productService.GetAllCategoriesAsync()
            };

            return View(pvm);
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductVM pvm)
        {
            if (ModelState.IsValid)
            {
                var result = await _productService.CreateProductAsync(pvm.Product.Name,
                    pvm.Product.Description,
                    pvm.Product.Price,
                    pvm.Product.ImageUrl,
                    pvm.Product.CategoryId
                    );

                return result ? RedirectToAction(nameof(Index)) : BadRequest(pvm);
            }
            
            _logger.LogInformation("Model is not valid. Errors : \n {errors}", 
                string.Join(Environment.NewLine, 
                    ModelState.Values
                        .Where(v=>v.Errors.Any())
                        .Select(v=> 
                            $"{v.AttemptedValue} - {v.Errors.Select(e=> e.ErrorMessage).FirstOrDefault()?.ToString()}" 
                        ).ToList()
                ));


            pvm.Categories = await _productService.GetAllCategoriesAsync();
            return View(pvm);
        }

        // GET: Product/Edit/5
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

            var pvm = new ProductVM
            {
                Product = product,
                Categories = await _productService.GetAllCategoriesAsync()
            };

            return View(pvm);
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ProductVM pvm)
        {
            if (id != pvm.Product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _productService.UpdateProductAsync(pvm.Product);
                return RedirectToAction(nameof(Index));
            }

            _logger.LogInformation("Model is not valid. Errors : \n {errors}", 
               string.Join(Environment.NewLine, 
                   ModelState.Values
                       .Where(v=>v.Errors.Any())
                       .Select(v=> 
                           $"{v.AttemptedValue} - {v.Errors.Select(e=> e.ErrorMessage).FirstOrDefault()?.ToString()}" 
                       ).ToList()
                   ));

            pvm.Categories = await _productService.GetAllCategoriesAsync();
            return View(pvm);
        }

        // GET: Product/Delete/5
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
                Product = product
            };
            
            return View(productVm);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _productService.DeleteProductAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
