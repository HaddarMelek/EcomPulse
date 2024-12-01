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
            var categories = await _productService.GetAllCategoriesAsync();

            var categoryVMs = categories.Select(c => new CategoryVM 
            {
                Id = c.Id,      
                Name = c.Name   
            }).ToList();

            return View(categoryVMs);
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _productService.GetCategroyById(id.Value);
            if (category == null)
            {
                return NotFound();
            }

            var categoryVM = new CategoryVM
            {
                Id = category.Id,
                Name = category.Name
            };

            return View(categoryVM);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] CategoryVM categoryVM)
        {
            if (ModelState.IsValid)
            {
                await _productService.AddCategoryAsync(categoryVM.Name);
                return RedirectToAction(nameof(Index));
            }
            return View(categoryVM);
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _productService.GetCategroyById(id.Value);
            if (category == null)
            {
                return NotFound();
            }

            var categoryVM = new CategoryVM()
            {
                Id = category.Id,
                Name = category.Name
            };

            return View(categoryVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name")] CategoryVM categoryVM)
        {
            if (id != categoryVM.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var category = new CategoryVM
                {
                    Id = categoryVM.Id,
                    Name = categoryVM.Name
                };

                //await _productService.UpdateCategoryAsync(category);
                return RedirectToAction(nameof(Index));
            }

            return View(categoryVM);
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _productService.GetCategroyById(id.Value);
            if (category == null)
            {
                return NotFound();
            }

            var categoryVM = new CategoryVM()
            {
                Id = category.Id,
                Name = category.Name
            };

            return View(categoryVM);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            //await _productService.deletobehere(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
