using EcomPulse.Web.Data;
using EcomPulse.Web.Models;
using EcomPulse.Web.ViewModel.Product;
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
            try
            {
                var products = await _context.Products
                    .Include(p => p.Category)
                    .ToListAsync();
                _logger.LogInformation("Successfully retrieved all products.");
                return products;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving all products.");
                throw;
            }
        }

        public async Task<Product?> GetProductByIdAsync(Guid id)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync(p => p.Id == id);
                if (product == null)
                {
                    _logger.LogWarning("Product with Id {Id} not found.", id);
                }
                else
                {
                    _logger.LogInformation("Successfully retrieved product with Id {Id}.", id);
                }
                return product;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving product with Id {Id}.", id);
                throw;
            }
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            try
            {
                var categories = await _context.Categories.ToListAsync();
                _logger.LogInformation("Successfully retrieved all categories.");
                return categories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving all categories.");
                throw;
            }
        }

        public async Task<bool> CreateProductAsync(string name, string description, decimal price, string? imageUrl, Guid categoryId, CancellationToken cancellationToken = default)
        {
            try
            {
                var category = await _context.Categories.FindAsync(new object[] { categoryId }, cancellationToken);

                if (category == null)
                {
                    _logger.LogError("Category with Id {CategoryId} not found.", categoryId);
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

                _logger.LogInformation("Successfully created product {ProductName}.", name);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating product {ProductName}.", name);
                throw;
            }
        }

        public async Task UpdateProductAsync(Guid id, string name, string description, decimal price, string? imageUrl, Guid categoryId, string? categoryName, CancellationToken cancellationToken = default)
        {
            try
            {
                var product = await _context.Products.FindAsync(id, cancellationToken);

                if (product == null)
                {
                    _logger.LogWarning("Product with Id {Id} not found.", id);
                    throw new KeyNotFoundException("Product not found.");
                }

                product.Name = name;
                product.Description = description;
                product.Price = price;
                product.ImageUrl = imageUrl;
                product.CategoryId = categoryId;

                _context.Update(product);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully updated product {ProductId}.", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating product {ProductId}.", id);
                throw;
            }
        }

        public async Task DeleteProductAsync(Guid id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);

                if (product == null)
                {
                    _logger.LogWarning("Product with Id {Id} not found.", id);
                    return;
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted product with Id {Id}.", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting product with Id {Id}.", id);
                throw;
            }
        }

        public async Task<Category?> GetCategroyById(Guid id)
        {
            return await _context.Categories.FindAsync(id);
        }
        public async Task AddCategoryAsync(string? name)
        {
            try
            {
                var category = new Category
                {
                    Id = Guid.NewGuid(),
                    Name = name
                };

                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully added category {CategoryName}.", name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding category {CategoryName}.", name);
                throw;
            }
        }

        public async Task UpdateCategoryAsync(CategoryVM categoryVm, CancellationToken cancellationToken)
        {
            try
            {
                var category = await _context.Categories.FindAsync(new object[] { categoryVm.Id }, cancellationToken);

                if (category == null)
                {
                    _logger.LogWarning("Category with Id {Id} not found.", categoryVm.Id);
                    throw new KeyNotFoundException("Category not found.");
                }

                category.Name = categoryVm.Name;

                _context.Categories.Update(category);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully updated category {CategoryId}.", categoryVm.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating category {CategoryId}.", categoryVm.Id);
                throw;
            }
        }

        public async Task DeleteCategoryAsync(Guid id)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);

                if (category == null)
                {
                    _logger.LogWarning("Category with Id {Id} not found.", id);
                    return;
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted category with Id {Id}.", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting category with Id {Id}.", id);
                throw;
            }
        }
    }
}
