using System;
using System.Threading.Tasks;
using EcomPulse.Web.Services;
using EcomPulse.Web.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EcomPulse.Web.Controllers
{
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
            return View(orders);
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();
            var order = await _orderService.GetOrderByIdAsync(id.Value);
            if (order == null) return NotFound();
            return View(order);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderVM orderVm)
        {
            if (!ModelState.IsValid) return View(orderVm);
            await _orderService.AddOrderAsync(orderVm);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();
            var order = await _orderService.GetOrderByIdAsync(id.Value);
            if (order == null) return NotFound();
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, OrderVM orderVm)
        {
            if (id != orderVm.Id) return NotFound();
            if (!ModelState.IsValid) return View(orderVm);
            await _orderService.UpdateOrderAsync(orderVm);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();
            var order = await _orderService.GetOrderByIdAsync(id.Value);
            if (order == null) return NotFound();
            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _orderService.DeleteOrderAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
