using System;
using System.Threading.Tasks;
using EcomPulse.Web.Services;
using EcomPulse.Web.ViewModel;
using EcomPulse.Web.ViewModel.User;
using EcomPulse.Web.ViewModel.Product;
using Microsoft.AspNetCore.Mvc;

namespace EcomPulse.Web.Controllers
{
    [Route("Admin/Dashboard")]
    public class AdminController : Controller
    {
        private readonly ProductService _productService;
        private readonly OrderService _orderService;

        public AdminController(ProductService productService, OrderService orderService)
        {
            _productService = productService;
            _orderService = orderService;
        }
        
        [HttpGet("")]
        public IActionResult Index()
        {
            return View("Dashboard"); 
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            var productVMs = products.Select(product => new ProductVM
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId
            }).ToList();
            return View(productVMs);
        }

        [HttpGet("products/{id}")]
        public async Task<IActionResult> GetProductDetails(Guid id)
        {
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            var productVm = new ProductVM
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId
            };

            return View(productVm);
        }


        [HttpPost("products")]
        public async Task<IActionResult> CreateProduct([FromForm] ProductVM productVm)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var success = await _productService.CreateProductAsync(
                productVm.Name, 
                productVm.Description, 
                productVm.Price, 
                productVm.ImageUrl, 
                productVm.CategoryId);

            if (!success) return BadRequest("Failed to create product");
            return RedirectToAction("GetProducts");
        }

        [HttpPost("products/update")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromForm] ProductVM productVm)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _productService.UpdateProductAsync(
                id, 
                productVm.Name, 
                productVm.Description, 
                productVm.Price, 
                productVm.ImageUrl, 
                productVm.CategoryId, 
                null);

            return RedirectToAction("GetProducts");
        }

        [HttpPost("products/delete/{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            await _productService.DeleteProductAsync(id);
            return RedirectToAction("GetProducts");
        }


        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _productService.GetAllCategoriesAsync();

            var categoryVms = categories.Select(c => new CategoryVM
            {
                Id = c.Id,
                Name = c.Name
            }).ToList();

            return View(categoryVms);
        }


        [HttpPost("categories/add")]
        public async Task<IActionResult> AddCategory([FromForm] CategoryVM categoryVm)
        {
            if (string.IsNullOrEmpty(categoryVm.Name))
            {
                ModelState.AddModelError("Name", "Category name is required.");
                return View("GetCategories");  
            }

            await _productService.AddCategoryAsync(categoryVm.Name);

            return RedirectToAction("GetCategories");
        }

        [HttpPost("categories/update")]
        public async Task<IActionResult> UpdateCategory([FromForm] CategoryVM categoryVm)
        {
            await _productService.UpdateCategoryAsync(categoryVm, default);
            return RedirectToAction("GetCategories");
        }

        [HttpPost("categories/delete/{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            await _productService.DeleteCategoryAsync(id);
            return RedirectToAction("GetCategories");
        }


        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return View(orders);
        }

        [HttpGet("orders/{id}")]
        public async Task<IActionResult> GetOrderDetails(Guid id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();

            var orderVm = new OrderVM
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
            };

            return View(orderVm);
        }


        [HttpPost("orders/delete/{id}")]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            await _orderService.DeleteOrderAsync(id);

            return RedirectToAction("GetOrders");
        }

    }
}
