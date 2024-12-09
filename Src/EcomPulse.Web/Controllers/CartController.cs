using System;
using System.Threading.Tasks;
using EcomPulse.Web.Services;
using EcomPulse.Web.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EcomPulse.Web.Controllers;

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
        if (id == null) return NotFound();

        try
        {
            var cart = await _cartService.GetCartByIdAsync(id.Value);
            if (cart == null) return NotFound();

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
    public async Task<IActionResult> UpdateQuantity(Guid productId, int newQuantity)
    {
        try
        {
            if (newQuantity < 1) newQuantity = 1;

            var success = await _cartService.UpdateCartItemQuantityAsync(User, productId, newQuantity);

            if (success)
            {
                _logger.LogInformation($"Updated quantity for ProductId {productId} to {newQuantity}.");
                return RedirectToAction(nameof(Index));
            }
            else
            {
                _logger.LogWarning($"Failed to update quantity for ProductId {productId}.");
                return View("Error");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating quantity for ProductId {productId}: {ex.Message}");
            return View("Error");
        }
    }
    [HttpPost]
    public async Task<IActionResult> RemoveItem(Guid productId)
    {
        try
        {
            var success = await _cartService.RemoveCartItemAsync(User, productId);

            if (success)
            {
                _logger.LogInformation($"Removed ProductId {productId} from the cart.");
                return RedirectToAction(nameof(Index));
            }
            else
            {
                _logger.LogWarning($"Failed to remove ProductId {productId}.");
                return View("Error");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error removing ProductId {productId} from the cart: {ex.Message}");
            return View("Error");
        }
    }

    public async Task<IActionResult> Create()
    {
        try
        {
            var cartVm = new CartVM();
            return View(cartVm);
        }
        catch (Exception ex)
        {
            return BadRequest();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CartVM cartVm)
    {
        if (ModelState.IsValid)
            try
            {
                var cart = await _cartService.CreateCartAsync(User);
                if (cart != null)
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

        return View(cartVm);
    }

   
    [HttpPost]
    [ValidateAntiForgeryToken]
    /*public async Task<IActionResult> Edit(Guid id, CartVM cartVm)
    {
        if (id != cartVm.Id) return NotFound();

        if (ModelState.IsValid)
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

        return View(cartVm);
    }
*/
    [HttpPost]
    [ActionName("Delete")]
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