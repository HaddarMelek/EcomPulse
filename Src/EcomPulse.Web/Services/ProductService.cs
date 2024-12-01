
using EcomPulse.Web.Data;
using EcomPulse.Web.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace EcomPulse.Web.Services
{
    public class ProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductService> _logger;

        public ProductService(ILogger<ProductService> logger, ApplicationDbContext context)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(Guid id )
        {
            
            return await _context.Products
                .Include(p=>p.Category)
                .FirstOrDefaultAsync(p=>p.Id==id);
            
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<bool> CreateProductAsync(string name,
            string description,
            decimal price,
            string? imageUrl,
            Guid categoryId, 
            CancellationToken cancellationToken = default)
        {
            var category = await _context.Categories.FindAsync(new object[] { categoryId }, cancellationToken);

            if (category == null)
            {
                _logger.LogError("Category with Id : {id} not found!", categoryId);
                return false;
            }

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = description,
                Price = price,
                ImageUrl = imageUrl,
                CategoryId = categoryId,
            };
            
            await _context.AddAsync(product, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation("Product created successfully");

            return true;
        }

        public async Task UpdateProductAsync(
            Guid id,
            string name,
            string description,
            decimal price,
            string? imageUrl,
            Guid categoryId, 
            string? categoryName,
            CancellationToken cancellationToken=default)
        {
            var product = await _context.Products.FindAsync(id, cancellationToken);
            
            if (product == null)
            {
                throw new KeyNotFoundException("Product not found");
            }
            product.Name = name;
            product.Description = description;
            product.Price = price;
            product.ImageUrl = imageUrl;
            product.CategoryId = categoryId;
            _context.Update(product);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteProductAsync(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Category?> GetCategroyById(Guid id)
        {
            return await _context.Categories.FindAsync(id);
        }
        
        public async Task AddCategoryAsync(string name)
        {
            var category = new Category
            {
                Id=Guid.NewGuid(),
                Name = name
            };
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }
    }
}
