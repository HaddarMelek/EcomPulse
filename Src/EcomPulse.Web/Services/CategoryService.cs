using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EcomPulse.Web.Models;
using Microsoft.EntityFrameworkCore;
using EcomPulse.Web.Data;

namespace EcomPulse.Web.Services;

public class CategoryService
{
    private readonly ApplicationDbContext _context;

    public CategoryService(ApplicationDbContext context)
    {
        _context = context;
    }

    // Fetch all categories
    public async Task<List<Category>> GetAllCategoriesAsync()
    {
        return await _context.Categories.ToListAsync();
    }

    // Fetch category by Id
    public async Task<Category> GetCategoryByIdAsync(Guid id)
    {
        return await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    // Add new category
    public async Task AddCategoryAsync(Category category)
    {
        category.Id = Guid.NewGuid();
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
    }

    // Update existing category
    public async Task UpdateCategoryAsync(Category category)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
    }

    // Delete category
    public async Task DeleteCategoryAsync(Guid id)
    {
        var category = await GetCategoryByIdAsync(id);
        if (category != null)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }
}
