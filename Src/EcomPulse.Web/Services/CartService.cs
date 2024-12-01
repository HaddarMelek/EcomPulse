// using EcomPulse.Web.Data;
// using EcomPulse.Web.Models;
// using EcomPulse.Web.ViewModel;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Logging;
//
// namespace EcomPulse.Web.Services
// {
//     public class CartService
//     {
//         private readonly ApplicationDbContext _context;
//         private readonly ILogger<CartService> _logger;
//
//         public CartService(ILogger<CartService> logger, ApplicationDbContext context)
//         {
//             _context = context;
//             _logger = logger;
//         }
//
//         // Get all carts
//         public async Task<List<CartVM>> GetAllCartsAsync()
//         {
//             return await _context.Cart
//                 .Include(c => c.User)
//                 .Include(c => c.CartItems)
//                     .ThenInclude(ci => ci.Product)
//                 .Select(c => new CartVM
//                 {
//                     CartId = c.Id,
//                     UserId = c.UserId,
//                     UserName = c.User.UserName,
//                     CartItems = c.CartItems.Select(ci => new CartItemVM
//                     {
//                         ProductId = ci.ProductId,
//                         ProductName = ci.Product.Name,
//                         ProductPrice = ci.Product.Price,
//                         Quantity = ci.Quantity
//                     }).ToList()
//                 })
//                 .ToListAsync();
//         }
//
//         public async Task<CartVM?> GetCartByIdAsync(Guid id)
//         {
//             var cart = await _context.Cart
//                 .Include(c => c.User)
//                 .Include(c => c.CartItems)
//                     .ThenInclude(ci => ci.Product)
//                 .Where(c => c.Id == id)
//                 .Select(c => new CartVM
//                 {
//                     CartId = c.Id,
//                     UserId = c.UserId,
//                     UserName = c.User.UserName,
//                     CartItems = c.CartItems.Select(ci => new CartItemVM
//                     {
//                         ProductId = ci.ProductId,
//                         ProductName = ci.Product.Name,
//                         ProductPrice = ci.Product.Price,
//                         Quantity = ci.Quantity
//                     }).ToList()
//                 })
//                 .FirstOrDefaultAsync();
//
//             return cart;
//         }
//
//         public async Task<bool> CreateCartAsync(CartVM cartVM)
//         {
//             var cartId = Guid.NewGuid();
//
//             await _context.Database.ExecuteSqlRawAsync(
//                 "INSERT INTO Cart (Id, UserId) VALUES ({0}, {1})",
//                 cartId,
//                 cartVM.UserId
//             );
//
//             foreach (var cartItemVM in cartVM.CartItems)
//             {
//                 await _context.Database.ExecuteSqlRawAsync(
//                     "INSERT INTO CartItems (Id, CartId, ProductId, Quantity) VALUES ({0}, {1}, {2}, {3})",
//                     Guid.NewGuid(),
//                     cartId,
//                     cartItemVM.ProductId,
//                     cartItemVM.Quantity
//                 );
//             }
//
//             return true;
//         }
//
//         public async Task AddCartAsync(CartVM cartVM)
//         {
//             var cartId = Guid.NewGuid();
//
//             await _context.Database.ExecuteSqlRawAsync(
//                 "INSERT INTO Cart (Id, UserId) VALUES ({0}, {1})",
//                 cartId,
//                 cartVM.UserId
//             );
//
//             foreach (var cartItemVM in cartVM.CartItems)
//             {
//                 await _context.Database.ExecuteSqlRawAsync(
//                     "INSERT INTO CartItems (Id, CartId, ProductId, Quantity) VALUES ({0}, {1}, {2}, {3})",
//                     Guid.NewGuid(),
//                     cartId,
//                     cartItemVM.ProductId,
//                     cartItemVM.Quantity
//                 );
//             }
//         }
//
//         public async Task<bool> UpdateCartAsync(CartVM cartVM)
//         {
//             var cart = await _context.Cart.FindAsync(cartVM.CartId);
//             if (cart == null)
//                 return false;
//
//             cart.UserId = cartVM.UserId;
//
//             var existingItems = _context.CartItem.Where(ci => ci.CartId == cartVM.CartId);
//             _context.CartItem.RemoveRange(existingItems);
//
//             foreach (var item in cartVM.CartItems)
//             {
//                 var cartItem = new CartItem()
//                 {
//                     CartId = cartVM.CartId,
//                     ProductId = item.ProductId,
//                     Quantity = item.Quantity
//                 };
//
//                 _context.CartItem.Add(cartItem);
//             }
//
//             await _context.SaveChangesAsync();
//             return true;
//         }
//
//         public async Task<bool> DeleteCartAsync(Guid id)
//         {
//             // var cart = await _context.Cart.FindAsync(id);
//             // if (cart == null) return false;
//             //
//             // var cartItems = _context.CartItem.Where(ci => ci.CartId == id);
//             // _context.CartItem.RemoveRange(cartItems);
//             //
//             // _context.Cart.Remove(cart);
//             // await _context.SaveChangesAsync();
//             return true;
//         }
//
//         public async Task<bool> CartExistsAsync(Guid id)
//         {
//             return await _context.Cart.AnyAsync(c => c.Id == id);
//         }
//     }
// }
