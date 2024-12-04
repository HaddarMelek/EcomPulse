// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using EcomPulse.Web.Data;
// using EcomPulse.Web.Models;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Logging;
//
// namespace EcomPulse.Web.Services;
//
// public class OrderService
// {
//     private readonly ApplicationDbContext _context;
//     private readonly ILogger<OrderService> _logger;
//
//     public OrderService(ILogger<OrderService> logger, ApplicationDbContext context)
//     {
//         _context = context;
//         _logger = logger;
//     }
//
//     public async Task<List<Order>> GetAllOrdersAsync()
//     {
//         try
//         {
//             var orders = await _context.Orders
//                 .Include(order => order.User)
//                 .Include(order => order.OrderItems)
//                 .ToListAsync();
//
//             _logger.LogInformation("Successfully fetched all orders.");
//             return orders;
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "Error fetching orders.");
//             throw;
//         }
//     }
//
//     public async Task<Order?> GetOrderByIdAsync(Guid id)
//     {
//         try
//         {
//             var order = await _context.Orders
//                 .Include(o => o.User)
//                 .Include(o => o.OrderItems)
//                 .FirstOrDefaultAsync(o => o.Id == id);
//
//             if (order == null)
//                 _logger.LogWarning("Order with Id {Id} not found.", id);
//             else
//                 _logger.LogInformation("Successfully fetched order with Id {Id}.", id);
//
//             return order;
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "Error fetching order.");
//             throw;
//         }
//     }
//
//     public async Task AddOrderAsync(Order order)
//     {
//         try
//         {
//             order.Id = Guid.NewGuid();
//             order.OrderDate = DateTime.UtcNow;
//
//             await _context.Orders.AddAsync(order);
//             await _context.SaveChangesAsync();
//
//             foreach (var orderItem in order.OrderItems)
//             {
//                 orderItem.Id = Guid.NewGuid();
//                 orderItem.OrderId = order.Id;
//
//                 await _context.OrderItems.AddAsync(orderItem);
//             }
//
//             await _context.SaveChangesAsync();
//             _logger.LogInformation("Order added successfully.");
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "Error adding order.");
//             throw;
//         }
//     }
//
//     public async Task UpdateOrderAsync(Guid id, Order updatedOrder)
//     {
//         try
//         {
//             var existingOrder = await _context.Orders
//                 .Include(o => o.OrderItems)
//                 .FirstOrDefaultAsync(o => o.Id == id);
//
//             if (existingOrder == null)
//             {
//                 _logger.LogError($"Order with Id {id} not found.");
//                 throw new Exception("Order not found.");
//             }
//
//
//             existingOrder.UserId = updatedOrder.Id;
//             existingOrder.Total = updatedOrder.Total;
//             existingOrder.OrderDate = updatedOrder.OrderDate;
//             existingOrder.ShippingAddress = updatedOrder.ShippingAddress;
//             existingOrder.Status = updatedOrder.Status;
//
//             _context.OrderItems.RemoveRange(existingOrder.OrderItems);
//             await _context.SaveChangesAsync();
//
//             foreach (var orderItem in existingOrder.OrderItems)
//             {
//                 orderItem.Id = Guid.NewGuid();
//                 orderItem.OrderId = existingOrder.Id;
//
//                 await _context.OrderItems.AddAsync(orderItem);
//             }
//
//             _context.Orders.Update(existingOrder);
//             await _context.SaveChangesAsync();
//
//             _logger.LogInformation("Order updated successfully.");
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "Error updating order.");
//             throw;
//         }
//     }
//
//     public async Task DeleteOrderAsync(Guid id)
//     {
//         try
//         {
//             var order = await _context.Orders
//                 .Include(o => o.OrderItems)
//                 .FirstOrDefaultAsync(o => o.Id == id);
//
//             if (order != null)
//             {
//                 _context.OrderItems.RemoveRange(order.OrderItems);
//                 _context.Orders.Remove(order);
//                 await _context.SaveChangesAsync();
//
//                 _logger.LogInformation($"Order with Id {id} deleted successfully.");
//             }
//             else
//             {
//                 _logger.LogError($"Order with Id {id} not found.");
//             }
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "Error deleting order.");
//             throw;
//         }
//     }
// }