// using EcomPulse.Web.Services;
// using EcomPulse.Web.ViewModel;
// using Microsoft.AspNetCore.Mvc;
//
// namespace EcomPulse.Web.Controllers
// {
//     public class OrderController : Controller
//     {
//         private readonly OrderService _orderService;
//
//         public OrderController(OrderService orderService)
//         {
//             _orderService = orderService;
//         }
//
//         public async Task<IActionResult> Index()
//         {
//             var orders = await _orderService.GetAllOrdersAsync();
//             return View(orders);
//         }
//
//         public async Task<IActionResult> Details(Guid id)
//         {
//             var order = await _orderService.GetOrderByIdAsync(id);
//             if (order == null)
//             {
//                 return NotFound();
//             }
//             return View(order);
//         }
//
//         public IActionResult Create()
//         {
//             return View(new OrderVM());
//         }
//
//         [HttpPost]
//         [ValidateAntiForgeryToken]
//         public async Task<IActionResult> Create(OrderVM orderVm)
//         {
//             if (ModelState.IsValid)
//             {
//                 await _orderService.AddOrderAsync(orderVm);
//                 return RedirectToAction(nameof(Index));
//             }
//             return View(orderVm);
//         }
//
//         public async Task<IActionResult> Edit(Guid id)
//         {
//             var order = await _orderService.GetOrderByIdAsync(id);
//             if (order == null)
//             {
//                 return NotFound();
//             }
//             return View(order);
//         }
//
//         [HttpPost]
//         [ValidateAntiForgeryToken]
//         public async Task<IActionResult> Edit(Guid id, OrderVM orderVm)
//         {
//             if (id != orderVm.Id)
//             {
//                 return NotFound();
//             }
//
//             if (ModelState.IsValid)
//             {
//                 await _orderService.UpdateOrderAsync(orderVm);
//                 return RedirectToAction(nameof(Index));
//             }
//             return View(orderVm);
//         }
//
//         public async Task<IActionResult> Delete(Guid id)
//         {
//             var order = await _orderService.GetOrderByIdAsync(id);
//             if (order == null)
//             {
//                 return NotFound();
//             }
//             return View(order);
//         }
//
//         [HttpPost, ActionName("Delete")]
//         [ValidateAntiForgeryToken]
//         public async Task<IActionResult> DeleteConfirmed(Guid id)
//         {
//             await _orderService.DeleteOrderAsync(id);
//             return RedirectToAction(nameof(Index));
//         }
//     }
// }
