
using EcomPulse.Web.Data;
using EcomPulse.Web.Models;
using EcomPulse.Web.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcomPulse.Web.Services
{
    public class ProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ApplicationDbContext> _logger;

        public ProductService(ILogger<ApplicationDbContext> logger, ApplicationDbContext context)
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

        public async Task<Product?> GetProductByIdAsync(Guid id, CancellationToken token=default)
        {
            return await _context.Products.FirstOrDefaultAsync(m => m.Id == id, token);
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<bool> CreateProductAsync(string name,
            string description,
            decimal price,
            string? imageUrl,
            Guid? categoryId, 
            CancellationToken cancellationToken = default)
        {
            var category = await _context.Categories.FindAsync(categoryId, cancellationToken);
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
                CategoryId = categoryId
            };
            
            await _context.AddAsync(product, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            
            _logger.LogError("Product created successfully");

            return true;
        }

        public async Task UpdateProductAsync(Product product)
        {
            _context.Update(product);
            await _context.SaveChangesAsync();
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
    }
}
