using EcomPulse.Web.ViewModel.Product;
using EcomPulse.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace EcomPulse.Web.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ProductService _productService;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(
            ILogger<CategoryController> logger, 
            ProductService productService)
        {
            _productService = productService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var categories = await _productService.GetAllCategoriesAsync();

                _logger.LogInformation("Fetched {Count} categories successfully.", categories.Count);

                var categoryVMs = categories.Select(c => new CategoryVM 
                {
                    Id = c.Id,      
                    Name = c.Name   
                }).ToList();

                return View(categoryVMs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching categories.");
                return View("Error");
            }
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                _logger.LogError("Details called with null ID.");
                return NotFound();
            }

            try
            {
                var category = await _productService.GetCategroyById(id.Value);

                if (category == null)
                {
                    _logger.LogWarning("Category with ID {Id} not found.", id.Value);
                    return NotFound();
                }

                var categoryVm = new CategoryVM
                {
                    Id = category.Id,
                    Name = category.Name
                };

                _logger.LogInformation("Fetched details for category with ID {Id}.", id.Value);

                return View(categoryVm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching category details for ID {Id}.", id);
                return View("Error");
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] CategoryVM categoryVm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _productService.AddCategoryAsync(categoryVm.Name);
                    _logger.LogInformation("Category {Name} created successfully.", categoryVm.Name);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while creating category {Name}.", categoryVm.Name);
                    return View("Error");
                }
            }
            return View(categoryVm);
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                _logger.LogError("Edit called with null ID.");
                return NotFound();
            }

            try
            {
                var category = await _productService.GetCategroyById(id.Value);

                if (category == null)
                {
                    _logger.LogWarning("Category with ID {Id} not found for edit.", id.Value);
                    return NotFound();
                }

                var categoryVm = new CategoryVM()
                {
                    Id = category.Id,
                    Name = category.Name
                };

                _logger.LogInformation("Fetched category with ID {Id} for edit.", id.Value);

                return View(categoryVm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching category for edit with ID {Id}.", id);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name")] CategoryVM categoryVm)
        {
            CancellationToken cancellationToken = default;

            if (id != categoryVm.Id)
            {
                _logger.LogError("Edit called with mismatched ID {Id}.", id);
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _productService.UpdateCategoryAsync(categoryVm, cancellationToken);
                    _logger.LogInformation("Category with ID {Id} updated successfully.", id);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while updating category with ID {Id}.", id);
                    return View("Error");
                }
            }

            return View(categoryVm);
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                _logger.LogError("Delete called with null ID.");
                return NotFound();
            }

            try
            {
                var category = await _productService.GetCategroyById(id.Value);

                if (category == null)
                {
                    _logger.LogWarning("Category with ID {Id} not found for deletion.", id.Value);
                    return NotFound();
                }

                var categoryVm = new CategoryVM()
                {
                    Id = category.Id,
                    Name = category.Name
                };

                _logger.LogInformation("Fetched category with ID {Id} for deletion.", id.Value);

                return View(categoryVm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching category for deletion with ID {Id}.", id);
                return View("Error");
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                await _productService.DeleteCategoryAsync(id);
                _logger.LogInformation("Category with ID {Id} deleted successfully.", id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting category with ID {Id}.", id);
                return View("Error");
            }
        }
    }
}
