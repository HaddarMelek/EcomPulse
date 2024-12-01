
using EcomPulse.Web.Data;
using EcomPulse.Web.Models;
using EcomPulse.Web.Services;
using EcomPulse.Web.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace EcomPulse.Web.Tests
{
    public class CartServiceTests
    {
        private readonly Mock<ILogger<CartService>> _mockLogger;
        private readonly ApplicationDbContext _context;
        private readonly CartService _cartService;

        public CartServiceTests()
        {
            // Setup in-memory database for testing
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("EcomPulseDb")
                .Options;
            _context = new ApplicationDbContext(options);

            _mockLogger = new Mock<ILogger<CartService>>();

            // Seed data
            SeedDatabase();

            // Create CartService instance
            _cartService = new CartService(_mockLogger.Object, _context);
        }

        private void SeedDatabase()
        {
            // Seed some data for testing
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = "testUser",
                Email = "test@gmail.com",
                Password = "password",
                Orders = null,
                Carts = null
            };
            var product = new Product { Id = Guid.NewGuid(), Name = "Test Product", Price = 10.0m };
            _context.Users.Add(user);
            _context.Products.Add(product);
            _context.SaveChanges();

            var cart = new Cart
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                User = null,
                CartItems = null
            };
            var cartItem = new CartItem
            {
                CartId = cart.Id,
                ProductId = product.Id,
                Quantity = 2,
                Cart = null,
                Product = null
            };
            _context.Carts.Add(cart);
            _context.CartItems.Add(cartItem);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAllCartsAsync_ShouldReturnCarts()
        {
            // Act
            var result = await _cartService.GetAllCartsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result); // We seeded one cart
            Assert.Equal(1, result.Count); 
        }

        [Fact]
        public async Task GetCartByIdAsync_ShouldReturnCart_WhenCartExists()
        {
            // Arrange
            var cartId = _context.Carts.First().Id;

            // Act
            var result = await _cartService.GetCartByIdAsync(cartId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cartId, result.CartId);
        }

        [Fact]
        public async Task GetCartByIdAsync_ShouldReturnNull_WhenCartDoesNotExist()
        {
            // Act
            var result = await _cartService.GetCartByIdAsync(Guid.NewGuid()); // Non-existing cart ID

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateCartAsync_ShouldCreateCartSuccessfully()
        {
            // Arrange
            var cartVm = new CartVM
            {
                UserId = _context.Users.First().Id,
                UserName = "testUser",
                CartItems = new List<CartItemVM>
                {
                    new CartItemVM
                    {
                        ProductId = _context.Products.First().Id,
                        ProductName = "Test Product",
                        ProductPrice = 10.0m,
                        Quantity = 1
                    }
                }
            };

            // Act
            var result = await _cartService.CreateCartAsync(cartVm);

            // Assert
            Assert.True(result);
            Assert.Equal(2, _context.Carts.Count()); // Ensure a new cart was created
            Assert.Equal(1, _context.CartItems.Count()); // Ensure the cart item was created
        }

        [Fact]
        public async Task UpdateCartAsync_ShouldUpdateCartSuccessfully()
        {
            // Arrange
            var cart = _context.Carts.First();
            var updatedCartVm = new CartVM
            {
                CartId = cart.Id,
                UserId = _context.Users.First().Id,
                UserName = "updatedUser",
                CartItems = new List<CartItemVM>
                {
                    new CartItemVM
                    {
                        ProductId = _context.Products.First().Id,
                        ProductName = "Test Product",
                        ProductPrice = 20.0m,
                        Quantity = 3
                    }
                }
            };

            // Act
            var result = await _cartService.UpdateCartAsync(updatedCartVm);

            // Assert
            Assert.True(result);
            var updatedCart = _context.Carts.First(c => c.Id == cart.Id);
            Assert.Equal(3, _context.CartItems.Count()); // Ensure the item quantity was updated
        }

        [Fact]
        public async Task DeleteCartAsync_ShouldDeleteCartSuccessfully()
        {
            // Arrange
            var cartId = _context.Carts.First().Id;

            // Act
            var result = await _cartService.DeleteCartAsync(cartId);

            // Assert
            Assert.True(result);
            Assert.Empty(_context.Carts); // Ensure the cart was deleted
        }

        [Fact]
        public async Task CartExistsAsync_ShouldReturnTrue_WhenCartExists()
        {
            // Arrange
            var cartId = _context.Carts.First().Id;

            // Act
            var result = await _cartService.CartExistsAsync(cartId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task CartExistsAsync_ShouldReturnFalse_WhenCartDoesNotExist()
        {
            // Act
            var result = await _cartService.CartExistsAsync(Guid.NewGuid());

            // Assert
            Assert.False(result);
        }
    }
}
