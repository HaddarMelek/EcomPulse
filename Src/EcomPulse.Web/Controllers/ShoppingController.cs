using EcomPulse.Web.Services;
using EcomPulse.Web.ViewModel;
using EcomPulse.Web.ViewModel.Product;
using EcomPulse.Web.ViewModel.Shopping;
using Microsoft.AspNetCore.Mvc;

namespace EcomPulse.Web.Controllers;

public class ShoppingController : Controller
{
    private readonly ProductService _productService;
    private readonly CartService _cartService;

    public ShoppingController(ProductService productService, CartService cartService)
    {
        _productService = productService;
        _cartService = cartService;
    }
    // GET
    
    [HttpGet]
    public async Task<IActionResult> Index(Guid? categoryId, decimal? minPrice, decimal? maxPrice)
    {
        var products = await _productService.GetAllProductsAsync();
        if (minPrice.HasValue)
            products = products.Where(p => p.Price >= minPrice.Value).ToList();

        if (maxPrice.HasValue)
            products = products.Where(p => p.Price <= maxPrice.Value).ToList();

        if (categoryId.HasValue)
            products = products.Where(p => p.CategoryId == categoryId.Value).ToList();

        var categories = await _productService.GetAllCategoriesAsync();
        var shoppingModel = new ShoppingVM
        {
            Categories = categories.Select(cat => new CategoryVM
            {
                Id = cat.Id,
                Name = cat.Name
            }).ToList(),
            Products = products.Select(prod => new ProductVM
            {
                Id = prod.Id,
                Name = prod.Name,
                Price = prod.Price,
                Description = prod.Description,
                CategoryId = prod.CategoryId
            }).ToList(),
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            

        };
        return View(shoppingModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddProductToCart(Guid productId, decimal productPrice)
    {
        try
        {
            await _cartService.AddProductToCartAsync(
                productId,
                productPrice,
                User);
            // Return success response
            return Json(new { success = true, message = "Product added to cart successfully!" });
        }
        catch (Exception ex)
        {
            // Log the error and return failure response
            return Json(new { success = false, message = "An error occurred while adding the product to the cart." });
        }
    }
    
    public async Task<IActionResult> UserCart()
    {
        try
        {
            var cart = await _cartService.GetCartForUserAsync(User);
            if (cart == null)
            {
                //_logger.LogWarning("No cart found for the current user.");
                return View(new CartVM()); 
            }

            var cartVm = new CartVM
            {
                Id = cart.Id,
                CartItems = cart.CartItems.Select(cartItem => new CartItemVM
                {
                    ProductId = cartItem.ProductId,
                    ProductName = cartItem.Product.Name,
                    ProductPrice = cartItem.ProductPrice,
                    Quantity = cartItem.Quantity
                }).ToList()
            };

           // _logger.LogInformation("Successfully retrieved the cart for the current user.");
            return View(cartVm);
        }
        catch (Exception ex)
        {
            //_logger.LogError($"Error retrieving the cart: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

}