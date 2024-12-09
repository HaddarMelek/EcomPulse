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
                .Include(c => c.User)
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

    public async Task<Cart?> CreateCartAsync( ClaimsPrincipal currentUser)
    {
        try
        {
            var user = await _userManager.GetUserAsync(currentUser);
            var cart = new Cart
            {
                Id = Guid.NewGuid(),
                User = user
            };
            cart =( await _context.Carts.AddAsync(cart)).Entity;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Cart created successfully.");
            return cart;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating cart: {ex.Message}");
            throw;
        }
    }

    public async Task<Cart> GetCartByUserAsync(ClaimsPrincipal currentUser)
    {
        var user = await _userManager.GetUserAsync(currentUser);
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.User.Id == user.Id);
        return cart;
    }
    
    private async Task<Cart> GetOrCreateCartForUserAsync(ClaimsPrincipal currentUser)
    {
        try
        {
            var cart = await GetCartByUserAsync(currentUser);
            if (cart == null)
            {
                cart = await CreateCartAsync(currentUser);
            }
            
            _logger.LogInformation($"Created a new cart for user {currentUser.Identity.Name}.");
            return cart;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in GetOrCreateCartForUserAsync: {ex.Message}");
            throw;
        }
    }

    public async Task AddProductToCartAsync(
        Guid productId, 
        decimal productPrice, 
        ClaimsPrincipal user)
    {
        var cart = await GetOrCreateCartForUserAsync(user);
        
        // Check if the product already exists in the cart
        var cartItem = cart.CartItems.FirstOrDefault(item => item.ProductId == productId);

        // If product exists, increase the quantity
        if (cartItem != null)
        {
            cartItem.Quantity++;  // Increase quantity by 1
            _context.CartItems.Update(cartItem);
        }
        else
        {
            // If product does not exist in the cart, add it as a new item
             cartItem = new CartItem
            {
                CartId = cart.Id,
                ProductId = productId,
                Quantity = 1,  // Start with quantity of 1
                ProductPrice = productPrice
            };
            await _context.CartItems.AddAsync(cartItem);
        }
         await _context.SaveChangesAsync();
    }

    public async Task<bool> UpdateCartAsync(Cart cart)
    {
        try
        {
            var existingCart = await _context.Carts.Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.Id == cart.Id);

            if (existingCart == null)
            {
                _logger.LogWarning($"Cart with ID {cart.Id} not found.");
                return false;
            }
            _context.CartItems.RemoveRange(existingCart.CartItems);

            foreach (var item in cart.CartItems)
            {
                var cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                };
                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Cart {cart.Id} updated successfully.");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating cart {cart.Id}: {ex.Message}");
            throw;
        }
    }
    public async Task<bool> UpdateCartItemQuantityAsync(ClaimsPrincipal currentUser, Guid productId, int quantity)
    {
        var user = await _userManager.GetUserAsync(currentUser);

        if (user == null) return false;

        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.User.Id == user.Id);

        var item = cart?.CartItems.FirstOrDefault(i => i.ProductId == productId);
        if (item == null) return false;

        item.Quantity = quantity;
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<bool> RemoveCartItemAsync(ClaimsPrincipal currentUser, Guid productId)
    {
        try
        {
            var user = await _userManager.GetUserAsync(currentUser);

            if (user == null) return false;

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.User.Id == user.Id);

            if (cart == null) return false;

            var cartItem = cart.CartItems.FirstOrDefault(item => item.ProductId == productId);

            if (cartItem == null) return false;

            cart.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error removing cart item: {ex.Message}");
            return false;
        }
    }
    public async Task<Cart?> GetCartForUserAsync(ClaimsPrincipal currentUser)
    {
        try
        {
            var user = await _userManager.GetUserAsync(currentUser);
            if (user == null)
            {
                _logger.LogWarning("No user found for the current ClaimsPrincipal.");
                return null;
            }

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product) 
                .FirstOrDefaultAsync(c => c.User.Id == user.Id);

            if (cart == null)
            {
                _logger.LogInformation("No cart found for the user with ID {UserId}.", user.Id);
            }

            return cart;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving the cart for the current user.");
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