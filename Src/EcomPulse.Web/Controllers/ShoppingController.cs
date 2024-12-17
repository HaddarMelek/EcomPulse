using EcomPulse.Web.Services;
using EcomPulse.Web.ViewModel;
using EcomPulse.Web.ViewModel.Product;
using EcomPulse.Web.ViewModel.Shopping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcomPulse.Web.Controllers;

[Authorize]
public class ShoppingController : Controller
{
    private readonly ProductService _productService;
    private readonly CartService _cartService;
    private readonly OrderService _orderService;
    private readonly ILogger<ShoppingController> _logger;


    public ShoppingController(ProductService productService,
        CartService cartService,
        OrderService orderService,
        ILogger<ShoppingController> logger)
    {
        _productService = productService;
        _orderService = orderService;
        _cartService = cartService;
        _logger = logger;
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
                _logger.LogWarning("No cart found for the current user.");
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

            _logger.LogInformation("Successfully retrieved the cart for the current user.");
            return View(cartVm);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving the cart: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateQuantity(Guid cartId, Guid productId, int incDec)
    {
        try
        {
            var cart = await _cartService.UpdateCartItemQuantityAsync(cartId, productId, incDec);
            return Json(new CartVM()
            {
                Id = cart.Id,
                CartItems = cart.CartItems.Select(item =>
                    new CartItemVM()
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        ProductName = item.Product.Name,
                        ProductPrice = item.ProductPrice
                    }).ToList(),
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating quantity for ProductId {productId}: {ex.Message}");
            return Json(new { error = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveProduct(Guid cartId, Guid productId)
    {
        try
        {
            var cart = await _cartService.RemoveProductFromCartAsync(cartId, productId);
            return PartialView("UserCart", new CartVM()
            {
                Id = cart.Id,
                CartItems = cart.CartItems.Select(item =>
                    new CartItemVM()
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        ProductName = item.Product.Name,
                        ProductPrice = item.ProductPrice
                    }).ToList(),
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating quantity for ProductId {productId}: {ex.Message}");
            return Json(new { error = ex.Message });
        }
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UserOrder(Guid cartId, string shippingAddress)
    {
        if (string.IsNullOrWhiteSpace(shippingAddress))
        {
            TempData["ErrorMessage"] = "Shipping address is required.";
            return View("UserCart"); // Returning to the cart view (change as needed)
        }

        try
        {
            // Retrieve the cart by ID.
            var cart = await _cartService.GetCartByIdAsync(cartId);
            if (cart == null || !cart.CartItems.Any())
            {
                TempData["ErrorMessage"] = "Cart is empty or invalid.";
                return View("UserCart"); // Returning to the cart view (change as needed)
            }

            var orderVm = new OrderVM
            {
                ShippingAddress = shippingAddress,
                OrderDate = DateTime.UtcNow,
                OrderItems = cart.CartItems.Select(ci => new OrderItemVM
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    Price = ci.ProductPrice
                }).ToList()
            };

            var currentUser = User;

            var orderId = await _orderService.CreateOrderAsync(orderVm, currentUser);

            if (orderId != null)
            {
                await _cartService.DeleteCartAsync(cartId);

                return RedirectToAction("Payment", "Order", new { orderId = orderId });
            }

            TempData["ErrorMessage"] = "Failed to place the order.";
            return View("UserCart"); // Returning to the cart view in case of failure
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error validating cart and creating order: {ex.Message}");
            TempData["ErrorMessage"] = "An error occurred while processing your order.";
            return View("UserCart"); // Returning to the cart view in case of an error
        }
    }


    [HttpGet]
    public async Task<IActionResult> Payment(Guid orderId)
    {
        try
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null) return NotFound("Order not found.");

            var model = new OrderVM()
            {
                Id = order.Id,
                OrderItems = order.OrderItems.Select(oi => new OrderItemVM()
                {
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity,
                    Price = oi.Price
                }).ToList()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving payment details: {ex.Message}");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}