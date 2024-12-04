using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using EcomPulse.Web.Services;
using EcomPulse.Web.ViewModel;
using System;
using System.Threading.Tasks;

namespace EcomPulse.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly CartService _cartService;
        private readonly ILogger<CartController> _logger;

        public CartController(CartService cartService, ILogger<CartController> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var carts = await _cartService.GetAllCartsAsync();
                _logger.LogInformation("Successfully retrieved all carts.");
                return View(carts);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving carts: {ex.Message}");
                return View("Error");
            }
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var cart = await _cartService.GetCartByIdAsync(id.Value);
                if (cart == null)
                {
                    return NotFound();
                    
                }

                _logger.LogInformation($"Successfully retrieved cart details for CartId: {id}");
                return View(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving cart details: {ex.Message}");
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CartVM cartVm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var success = await _cartService.CreateCartAsync(cartVm);
                    if (success)
                    {
                        _logger.LogInformation("Cart created successfully.");
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        _logger.LogWarning("Failed to create cart.");
                        return View("Error");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error creating cart: {ex.Message}");
                    return View("Error");
                }
            }
            return View(cartVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, CartVM cartVm)
        {
            if (id != cartVm.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var success = await _cartService.UpdateCartAsync(cartVm);
                    if (success)
                    {
                        _logger.LogInformation("Cart updated successfully.");
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        _logger.LogWarning("Failed to update cart.");
                        return View("Error");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error updating cart: {ex.Message}");
                    return View("Error");
                }
            }
            return View(cartVm);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var success = await _cartService.DeleteCartAsync(id);
                if (success)
                {
                    _logger.LogInformation($"Cart {id} deleted successfully.");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _logger.LogWarning($"Failed to delete cart {id}.");
                    return View("Error");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting cart: {ex.Message}");
                return View("Error");
            }
        }
    }
}
