using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EcomPulse.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace EcomPulse.Web.Data;

public class DataSeeder
{
    private readonly ApplicationDbContext _context;
    public DataSeeder(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SeedUser()
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == "mhaddar");
        if (user == null)
        {
            await _context.Users.AddAsync(new User
            {
                Id = Guid.NewGuid(),
                UserName = "mhaddar",
                Email = "test@test.tn",
                Password = "vvvv123432"
            });
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task CreateCartAndOrderAsync()
{
    var userId = Guid.Parse("2a2ab6dc-74b7-483f-a76f-a24fd4928a2a");

    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
    if (user == null)
    {
        throw new ArgumentNullException(nameof(user), "User with the specified ID does not exist.");
    }

    await using var transaction = await _context.Database.BeginTransactionAsync();

    try
    {
       
        var product = await _context.Products.FirstOrDefaultAsync();
        if (product == null)
        {
            throw new InvalidOperationException("No products available to add to the cart.");
        }
        var cart = new Cart
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            User = user,
            CartItems = null
        };

        var cartItem = new CartItem
        {
            Id = Guid.NewGuid(),
            CartId = cart.Id,
            Cart = cart,
            ProductId = product.Id,
            Product = product,
            Quantity = 2
        };

        cart.CartItems = new List<CartItem> { cartItem };
        await _context.Carts.AddAsync(cart);
        await _context.SaveChangesAsync();

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            User = user,
            ShippingAddress = "123 Main Street, Test City",
            OrderDate = DateTime.UtcNow,
            Total = cartItem.Quantity * product.Price,
            OrderItems = null
        };

        var orderItem = new OrderItem
        {
            Id = Guid.NewGuid(),
            OrderId = order.Id,
            Order = order,
            ProductId = product.Id,
            Product = product,
            Quantity = cartItem.Quantity,
            Price = product.Price
        };

        order.OrderItems = new List<OrderItem> { orderItem };
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();

        await transaction.CommitAsync();
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }
}

    
}