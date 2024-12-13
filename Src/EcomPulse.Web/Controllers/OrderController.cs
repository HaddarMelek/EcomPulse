using EcomPulse.Web.Models;
using EcomPulse.Web.Services;
using EcomPulse.Web.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace EcomPulse.Web.Controllers;

public class OrderController : Controller
{
    private readonly OrderService _orderService;
    private readonly ILogger<OrderController> _logger;

    public OrderController(OrderService orderService, ILogger<OrderController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        var ordersVm = orders.Select(order => new OrderVM
        {
            Id = order.Id,
            OrderDate = order.OrderDate,
            ShippingAddress = order.ShippingAddress,
            Status = order.Status,
            OrderItems = order.OrderItems.Select(item => new OrderItemVM
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.Product.Name,
                Quantity = item.Quantity,
                Price = item.Price,
            }).ToList()
        }).ToList();
        return View(ordersVm);
    }

    public async Task<IActionResult> Details(Guid? id)
    {
        if (id == null) return NotFound();
        var order = await _orderService.GetOrderByIdAsync(id.Value);
        if (order == null) return NotFound();
        var orderVm = new OrderVM();
        return View(orderVm);
    }

    public IActionResult Create()
    {
        try
        {
            var orderVm = new OrderVM();
            return View(orderVm);
        }
        catch (Exception ex)
        {
            return BadRequest();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(OrderVM orderVm)
    {
        if (ModelState.IsValid)
            try
            {
                var orderId = await _orderService.CreateOrderAsync(orderVm, User);
                if (orderId != null)
                {
                    _logger.LogInformation("Order created successfully.");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _logger.LogWarning("Failed to create Order.");
                    return View("Error");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating Order: {ex.Message}");
                return View("Error");
            }

        return View(orderVm);
    }

    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id == null) return NotFound();
        var order = await _orderService.GetOrderByIdAsync(id.Value);
        if (order == null) return NotFound();
        var orderVm = new OrderVM();
        return View(orderVm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, Order order)
    {
        if (id != order.Id) return NotFound();
        if (!ModelState.IsValid)
        {
            var orderVm = new OrderVM();
            return View(orderVm);
        }

        await _orderService.UpdateOrderAsync(order);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id == null) return NotFound();
        var order = await _orderService.GetOrderByIdAsync(id.Value);
        if (order == null) return NotFound();
        return View(order);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        await _orderService.DeleteOrderAsync(id);
        return RedirectToAction(nameof(Index));
    }


    public async Task<IActionResult> My()
    {
        try
        {
            var user = await _orderService.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var orders = await _orderService.GetOrdersByUserIdAsync(user.Id);
            var ordersVm = orders.Select(order => new OrderVM
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                ShippingAddress = order.ShippingAddress,
                Status = order.Status,
                OrderItems = order.OrderItems.Select(item => new OrderItemVM
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductName = item.Product.Name,
                    Quantity = item.Quantity,
                    Price = item.Price
                }).ToList()
            }).ToList();

            return View(ordersVm);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching user orders: {ex.Message}");
            return View("Error");
        }
    }

    public async Task<IActionResult> Payment(Guid orderId)
    {
        var order = await _orderService.GetOrderByIdAsync(orderId);
        if (order == null) return View("Error");
        var orderVm = new OrderVM
        {
            Id = order.Id,
            OrderDate = order.OrderDate,
            ShippingAddress = order.ShippingAddress,
            Status = order.Status,
            OrderItems = order.OrderItems.Select(item => new OrderItemVM
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.Product.Name,
                Quantity = item.Quantity,
                Price = item.Price
            }).ToList()
        };
        return View(orderVm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ValidatePayment(Guid orderId)
    {
        var order = await _orderService.GetOrderByIdAsync(orderId);

        if (order == null) return View("Error");

        order.Status = "In Progress";
        await _orderService.UpdateOrderAsync(order);

        return RedirectToAction("My");
    }
    public async Task<IActionResult> ViewOrderItems(Guid id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null) return View("Error");

        var orderItemsVm = order.OrderItems.Select(item => new OrderItemVM
        {
            Id = item.Id,
            ProductId = item.ProductId,
            ProductName = item.Product.Name,
            Quantity = item.Quantity,
            Price = item.Price
        }).ToList();

        var orderVm = new OrderVM
        {
            Id = order.Id,
            OrderDate = order.OrderDate,
            ShippingAddress = order.ShippingAddress,
            Status = order.Status,
            OrderItems = orderItemsVm
        };

        return View(orderVm);
    }

}