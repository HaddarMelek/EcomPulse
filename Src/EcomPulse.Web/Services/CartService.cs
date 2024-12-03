using EcomPulse.Web.Data;
using EcomPulse.Web.Models;
using EcomPulse.Web.ViewModel;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcomPulse.Web.Services
{
    public class CartService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CartService> _logger;

        public CartService(ILogger<CartService> logger, ApplicationDbContext context)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<CartVM>> GetAllCartsAsync()
        {
            try
            {
                var carts = await _context.Carts
                    .Include(c => c.User)
                    .Include(c => c.CartItems)
                        .ThenInclude(ci => ci.Product)
                    .Select(c => new CartVM
                    {
                        Id = c.Id,
                        UserId = c.UserId,
                        UserName = c.User.UserName,
                        CartItems = c.CartItems.Select(ci => new CartItemVM
                        {
                            ProductId = ci.ProductId,
                            ProductName = ci.Product.Name,
                            ProductPrice = ci.Product.Price,
                            Quantity = ci.Quantity
                        }).ToList()
                    })
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

        public async Task<CartVM?> GetCartByIdAsync(Guid id)
        {
            try
            {
                var cart = await _context.Carts
                    .Include(c => c.User)
                    .Include(c => c.CartItems)
                        .ThenInclude(ci => ci.Product)
                    .Where(c => c.Id == id)
                    .Select(c => new CartVM
                    {
                        Id = c.Id,
                        UserId = c.UserId,
                        UserName = c.User.UserName,
                        CartItems = c.CartItems.Select(ci => new CartItemVM
                        {
                            ProductId = ci.ProductId,
                            ProductName = ci.Product.Name,
                            ProductPrice = ci.Product.Price,
                            Quantity = ci.Quantity
                        }).ToList()
                    })
                    .FirstOrDefaultAsync();

                if (cart == null)
                {
                    _logger.LogWarning($"Cart with ID {id} not found.");
                }

                return cart;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving cart with ID {id}: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> CreateCartAsync(CartVM cartVM)
        {
            try
            {
                var cart = new Cart
                {
                    Id = Guid.NewGuid(),
                    UserId = cartVM.UserId,
                    User = null,
                    CartItems = null
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();

                foreach (var item in cartVM.CartItems)
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

                cart.UserId = cartVM.UserId;

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
}
