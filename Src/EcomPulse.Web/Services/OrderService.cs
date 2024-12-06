using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using EcomPulse.Web.Data;
using EcomPulse.Web.Models;
using EcomPulse.Web.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EcomPulse.Web.Services;

public class OrderService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<OrderService> _logger;
    private readonly UserManager<IdentityUser> _userManager;

    public OrderService(ILogger<OrderService> logger,
        ApplicationDbContext context,
        UserManager<IdentityUser> userManager)
    {
        _context = context;
        _logger = logger;
        _userManager = userManager;
    }

    public async Task<List<Order>> GetAllOrdersAsync()
    {
        try
        {
            var orders = await _context.Orders
                .Include(order => order.User)
                .Include(order => order.OrderItems)
                .ToListAsync();

            _logger.LogInformation("Successfully fetched all orders.");
            return orders;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching orders.");
            throw;
        }
    }

    public async Task<Order?> GetOrderByIdAsync(Guid id)
    {
        try
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                _logger.LogWarning("Order with Id {Id} not found.", id);
            else
                _logger.LogInformation("Successfully fetched order with Id {Id}.", id);

            return order;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching order.");
            throw;
        }
    }

    public async Task<bool> CreateOrderAsync(OrderVM orderVm, ClaimsPrincipal currentUser)
    {
        try
        {
            var user = await _userManager.GetUserAsync(currentUser);
            if (user == null) throw new Exception("User not logged in");
            var order = new Order
            {
                Id = Guid.NewGuid(),
                User = user,
                ShippingAddress = orderVm.ShippingAddress,
                OrderDate = orderVm.OrderDate
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var item in orderVm.OrderItems)
            {
                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price,
                };
                _context.OrderItems.Add(orderItem);
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Order created successfully.");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating Order: {ex.Message}");
            throw;
        }
    }

    public async Task UpdateOrderAsync(Guid id, Order updatedOrder)
    {
        try
        {
            var existingOrder = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (existingOrder == null)
            {
                _logger.LogError($"Order with Id {id} not found.");
                throw new Exception("Order not found.");
            }


            existingOrder.Total = updatedOrder.Total;
            existingOrder.OrderDate = updatedOrder.OrderDate;
            existingOrder.ShippingAddress = updatedOrder.ShippingAddress;
            existingOrder.Status = updatedOrder.Status;

            _context.OrderItems.RemoveRange(existingOrder.OrderItems);
            await _context.SaveChangesAsync();

            foreach (var orderItem in existingOrder.OrderItems)
            {
                orderItem.Id = Guid.NewGuid();
                orderItem.OrderId = existingOrder.Id;

                await _context.OrderItems.AddAsync(orderItem);
            }

            _context.Orders.Update(existingOrder);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Order updated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating order.");
            throw;
        }
    }

    public async Task DeleteOrderAsync(Guid id)
    {
        try
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order != null)
            {
                _context.OrderItems.RemoveRange(order.OrderItems);
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Order with Id {id} deleted successfully.");
            }
            else
            {
                _logger.LogError($"Order with Id {id} not found.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting order.");
            throw;
        }
    }
}