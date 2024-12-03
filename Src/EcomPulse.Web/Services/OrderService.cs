using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EcomPulse.Web.Data;
using EcomPulse.Web.Models;
using EcomPulse.Web.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EcomPulse.Web.Services
{
    public class OrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OrderService> _logger;

        public OrderService(ILogger<OrderService> logger, ApplicationDbContext context)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<OrderVM>> GetAllOrdersAsync()
        {
            try
            {
                var orders = await _context.Orders
                    .Include(order => order.User)
                    .Select(order => new OrderVM
                    {
                        Id = order.Id,
                        UserId = order.UserId,
                        Total = order.Total,
                        OrderDate = order.OrderDate,
                        ShippingAddress = order.ShippingAddress,
                        Status = order.Status,
                        OrderItems = order.OrderItems.Select(oi => new OrderItemVM
                        {
                            Id = oi.Id,
                            ProductId = oi.ProductId,
                            Quantity = oi.Quantity,
                            Price = oi.Price
                        }).ToList()
                    })
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

        public async Task<OrderVM?> GetOrderByIdAsync(Guid id)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.User)
                    .Where(o => o.Id == id)
                    .Select(o => new OrderVM
                    {
                        Id = o.Id,
                        UserId = o.UserId,
                        Total = o.Total,
                        OrderDate = o.OrderDate,
                        ShippingAddress = o.ShippingAddress,
                        Status = o.Status,
                        OrderItems = o.OrderItems.Select(oi => new OrderItemVM
                        {
                            Id = oi.Id,
                            ProductId = oi.ProductId,
                            Quantity = oi.Quantity,
                            Price = oi.Price
                        }).ToList()
                    })
                    .FirstOrDefaultAsync();
                _logger.LogInformation($"Successfully fetched order with Id {id}.");
                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching order.");
                throw;
            }
        }

        public async Task AddOrderAsync(OrderVM orderVm)
        {
            try
            {
                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = orderVm.UserId,
                    Total = orderVm.Total,
                    OrderDate = DateTime.UtcNow,
                    ShippingAddress = orderVm.ShippingAddress,
                    Status = orderVm.Status,
                    User = null,
                    OrderItems = null
                };

                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();

                foreach (var orderItemVm in orderVm.OrderItems)
                {
                    var orderItem = new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order.Id,
                        ProductId = orderItemVm.ProductId,
                        Quantity = orderItemVm.Quantity,
                        Price = orderItemVm.Price,
                        Order = null,
                        Product = null
                    };

                    await _context.OrderItems.AddAsync(orderItem);
                }
                await _context.SaveChangesAsync();

                _logger.LogInformation("Order added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding order.");
                throw;
            }
        }

        public async Task UpdateOrderAsync(OrderVM orderVm)
        {
            try
            {
                var order = await _context.Orders.FindAsync(orderVm.Id);
                if (order == null)
                {
                    _logger.LogError($"Order with Id {orderVm.Id} not found.");
                    throw new Exception("Order not found.");
                }

                order.UserId = orderVm.UserId;
                order.Total = orderVm.Total;
                order.OrderDate = orderVm.OrderDate;
                order.ShippingAddress = orderVm.ShippingAddress;
                order.Status = orderVm.Status;

                _context.Orders.Update(order);
                await _context.SaveChangesAsync();

                var existingOrderItems = await _context.OrderItems
                    .Where(oi => oi.OrderId == orderVm.Id)
                    .ToListAsync();
                _context.OrderItems.RemoveRange(existingOrderItems);
                await _context.SaveChangesAsync();

                foreach (var orderItemVm in orderVm.OrderItems)
                {
                    var orderItem = new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        OrderId = orderVm.Id,
                        ProductId = orderItemVm.ProductId,
                        Quantity = orderItemVm.Quantity,
                        Price = orderItemVm.Price,
                        Order = null,
                        Product = null
                    };

                    await _context.OrderItems.AddAsync(orderItem);
                }
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
                var order = await _context.Orders.FindAsync(id);
                if (order != null)
                {
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
}
