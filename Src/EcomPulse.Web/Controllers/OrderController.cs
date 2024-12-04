// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using EcomPulse.Web.Models;
// using EcomPulse.Web.Services;
// using EcomPulse.Web.ViewModel;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Logging;
//
// namespace EcomPulse.Web.Controllers;
//
// public class OrderController : Controller
// {
//     private readonly OrderService _orderService;
//     private readonly ILogger<OrderController> _logger;
//
//     public OrderController(OrderService orderService, ILogger<OrderController> logger)
//     {
//         _orderService = orderService;
//         _logger = logger;
//     }
//
//     public async Task<IActionResult> Index()
//     {
//         var orders = await _orderService.GetAllOrdersAsync();
//         var ordersVm = new List<OrderVM>();
//         return View(ordersVm);
//     }
//
//     public async Task<IActionResult> Details(Guid? id)
//     {
//         if (id == null) return NotFound();
//         var order = await _orderService.GetOrderByIdAsync(id.Value);
//         if (order == null) return NotFound();
//         var orderVm = new OrderVM();
//         return View(orderVm);
//     }
//
//     public IActionResult Create()
//     {
//         return View();
//     }
//
//     [HttpPost]
//     [ValidateAntiForgeryToken]
//     public async Task<IActionResult> Create(OrderVM orderVm)
//     {
//         if (!ModelState.IsValid) return View(orderVm);
//         var order = new Order
//         {
//             User = null,
//             ShippingAddress = null,
//             OrderItems = null
//         };
//         await _orderService.AddOrderAsync(order);
//         return RedirectToAction(nameof(Index));
//     }
//
//     public async Task<IActionResult> Edit(Guid? id)
//     {
//         if (id == null) return NotFound();
//         var order = await _orderService.GetOrderByIdAsync(id.Value);
//         if (order == null) return NotFound();
//         var orderVm = new OrderVM();
//         return View(orderVm);
//     }
//
//     [HttpPost]
//     [ValidateAntiForgeryToken]
//     public async Task<IActionResult> Edit(Guid id, Order order)
//     {
//         if (id != order.Id) return NotFound();
//         if (!ModelState.IsValid)
//         {
//             var orderVm = new OrderVM();
//             return View(orderVm);
//         }
//
//         await _orderService.UpdateOrderAsync(id, order);
//         return RedirectToAction(nameof(Index));
//     }
//
//     public async Task<IActionResult> Delete(Guid? id)
//     {
//         if (id == null) return NotFound();
//         var order = await _orderService.GetOrderByIdAsync(id.Value);
//         if (order == null) return NotFound();
//         return View(order);
//     }
//
//     [HttpPost]
//     [ActionName("Delete")]
//     [ValidateAntiForgeryToken]
//     public async Task<IActionResult> DeleteConfirmed(Guid id)
//     {
//         await _orderService.DeleteOrderAsync(id);
//         return RedirectToAction(nameof(Index));
//     }
// }