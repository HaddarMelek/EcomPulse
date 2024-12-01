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
            try
            {
                var products = await _productService.GetAllProductsAsync();
                var productVMs = products.Select(product => new ProductVM
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    ImageUrl = product.ImageUrl,
                    CategoryId = product.CategoryId,
                    CategoryName = product.Category.Name,

                   

                }).ToList();

                _logger.LogInformation("Retrieved {Count} products successfully.", products.Count);
                return View(productVMs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving products.");
                return StatusCode(500, "Internal server error");
            }
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Details called with null ID.");
                return NotFound();
            }

            try
            {
                var product = await _productService.GetProductByIdAsync(id.Value);
                if (product == null)
                {
                    _logger.LogWarning("Product with ID {Id} not found.", id);
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
                };

                _logger.LogInformation("Retrieved details for product ID {Id}.", id);
                return View(productVm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving details for product ID {Id}.", id);
                return StatusCode(500, "Internal server error");
            }
        }

        public async Task<IActionResult> Create()
        {
            try
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

                _logger.LogInformation("Prepared product creation view.");
                return View(pvm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while preparing the product creation view.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductVM productVm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _productService.CreateProductAsync(
                        productVm.Name,
                        productVm.Description,
                        productVm.Price,
                        productVm.ImageUrl,
                        productVm.CategoryId
                    );

                    if (result)
                    {
                        _logger.LogInformation("Product created successfully: {Name}.", productVm.Name);
                        return RedirectToAction(nameof(Index));
                    }

                    _logger.LogWarning("Failed to create product: {Name}.", productVm.Name);
                    return BadRequest(productVm);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while creating product: {Name}.", productVm.Name);
                    return StatusCode(500, "Internal server error");
                }
            }

            _logger.LogWarning("Invalid model state while creating product: {Name}.", productVm.Name);
            

            return View(productVm);
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Edit called with null ID.");
                return NotFound();
            }

            try
            {
                var product = await _productService.GetProductByIdAsync(id.Value);
                if (product == null)
                {
                    _logger.LogWarning("Product with ID {Id} not found.", id);
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

                _logger.LogInformation("Prepared edit view for product ID {Id}.", id);
                return View(productVm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while preparing the edit view for product ID {Id}.", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ProductVM productVm)
        {
            if (id != productVm.Id)
            {
                _logger.LogWarning("Edit called with mismatched IDs. Route ID: {RouteId}, Model ID: {ModelId}.", id, productVm.Id);
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
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

                    _logger.LogInformation("Updated product successfully: ID {Id}.", id);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while updating product: ID {Id}.", id);
                    return StatusCode(500, "Internal server error");
                }
            }

            _logger.LogWarning("Invalid model state while updating product: ID {Id}.", id);
            
            return View(productVm);
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Delete called with null ID.");
                return NotFound();
            }

            try
            {
                var product = await _productService.GetProductByIdAsync(id.Value);
                if (product == null)
                {
                    _logger.LogWarning("Product with ID {Id} not found.", id);
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

                   
                };

                _logger.LogInformation("Prepared delete view for product ID {Id}.", id);
                return View(productVm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while preparing the delete view for product ID {Id}.", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                await _productService.DeleteProductAsync(id);
                _logger.LogInformation("Deleted product successfully: ID {Id}.", id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting product: ID {Id}.", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
