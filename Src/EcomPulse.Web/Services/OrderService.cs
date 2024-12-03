using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EcomPulse.Web.Data;
using EcomPulse.Web.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NuGet.Packaging;

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
            return await _context.Orders
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
        }

        public async Task<OrderVM?> GetOrderByIdAsync(Guid id)
        {
            return await _context.Orders
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
        }

        public async Task AddOrderAsync(OrderVM orderVm)
        {
            var orderId = Guid.NewGuid();

            await _context.Database.ExecuteSqlRawAsync(
                "INSERT INTO Orders (Id, UserId, Total, OrderDate, ShippingAddress, Status) VALUES ({0}, {1}, {2}, {3}, {4}, {5})",
                orderId, 
                orderVm.UserId, 
                orderVm.Total, 
                DateTime.UtcNow, 
                orderVm.ShippingAddress, 
                orderVm.Status
            );

            foreach (var orderItemVm in orderVm.OrderItems)
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "INSERT INTO OrderItems (Id, OrderId, ProductId, Quantity, Price) VALUES ({0}, {1}, {2}, {3}, {4})",
                    Guid.NewGuid(), 
                    orderId, 
                    orderItemVm.ProductId, 
                    orderItemVm.Quantity, 
                    orderItemVm.Price
                );
            }
        }

        public async Task UpdateOrderAsync(OrderVM orderVm)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "UPDATE Orders SET UserId = {0}, Total = {1}, OrderDate = {2}, ShippingAddress = {3}, Status = {4} WHERE Id = {5}",
                orderVm.UserId,
                orderVm.Total,
                orderVm.OrderDate,
                orderVm.ShippingAddress,
                orderVm.Status,
                orderVm.Id
            );

            await _context.Database.ExecuteSqlRawAsync(
                "DELETE FROM OrderItems WHERE OrderId = {0}",
                orderVm.Id
            );

            foreach (var orderItemVm in orderVm.OrderItems)
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "INSERT INTO OrderItems (Id, OrderId, ProductId, Quantity, Price) VALUES ({0}, {1}, {2}, {3}, {4})",
                    Guid.NewGuid(),
                    orderVm.Id,
                    orderItemVm.ProductId,
                    orderItemVm.Quantity,
                    orderItemVm.Price
                );
            }
        }

        public async Task DeleteOrderAsync(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
        }
    }
}
