using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EcomPulse.Web.Data;
using EcomPulse.Web.Models;
using EcomPulse.Web.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EcomPulse.Web.Services;

public class CartService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CartService> _logger;
    private readonly UserManager<IdentityUser> _userManager;


    public CartService(ILogger<CartService> logger,
        ApplicationDbContext context,
        UserManager<IdentityUser> userManager)
    {
        _context = context;
        _logger = logger;
        _userManager = userManager;
    }

    public async Task<List<Cart>> GetAllCartsAsync()
    {
        try
        {
            var carts = await _context.Carts
                .Include(c => c.User)
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .ToListAsync();

            _logger.LogInformation("Retrieved all carts.");
            return carts;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving all carts: {ex.Message}");
            throw;
        }
    }

    public async Task<Cart?> GetCartByIdAsync(Guid id)
    {
        try
        {
            var cart = await _context.Carts
                //    .Include(c => c.User)
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cart == null) _logger.LogWarning($"Cart with ID {id} not found.");

            return cart;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving cart with ID {id}: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> CreateCartAsync(CartVM cartVm, ClaimsPrincipal currentUser)
    {
        try
        {
            var user = await _userManager.GetUserAsync(currentUser);
            if (user == null) throw new Exception("User not logged in");
            var cart = new Cart
            {
                Id = Guid.NewGuid(),
                User = user
            };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            foreach (var item in cartVm.CartItems)
            {
                var cartItem = new CartItem
                {
                    Id = Guid.NewGuid(),
                    CartId = cart.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Cart = null,
                    Product = null
                };
                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Cart created successfully.");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating cart: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> UpdateCartAsync(CartVM cartVM)
    {
        try
        {
            var cart = await _context.Carts.FindAsync(cartVM.Id);
            if (cart == null)
            {
                _logger.LogWarning($"Cart with ID {cartVM.Id} not found.");
                return false;
            }

            var existingItems = _context.CartItems.Where(ci => ci.CartId == cartVM.Id);
            _context.CartItems.RemoveRange(existingItems);

            foreach (var item in cartVM.CartItems)
            {
                var cartItem = new CartItem
                {
                    CartId = cartVM.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Cart = null,
                    Product = null
                };
                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Cart {cartVM.Id} updated successfully.");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating cart {cartVM.Id}: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> DeleteCartAsync(Guid id)
    {
        try
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
            {
                _logger.LogWarning($"Cart with ID {id} not found.");
                return false;
            }

            var cartItems = _context.CartItems.Where(ci => ci.CartId == id);
            _context.CartItems.RemoveRange(cartItems);

            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Cart {id} deleted successfully.");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting cart with ID {id}: {ex.Message}");
            throw;
        }
    }
}