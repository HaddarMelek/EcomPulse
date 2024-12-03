using System;
using System.Threading.Tasks;
using EcomPulse.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace EcomPulse.Web.Data;

public class DataSeeder
{
    private readonly ApplicationDbContext _context;
    public DataSeeder(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SeedUser()
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == "mhaddar");
        if (user == null)
        {
            await _context.Users.AddAsync(new User
            {
                Id = Guid.NewGuid(),
                UserName = "mhaddar",
                Email = "test@test.tn",
                Password = "vvvv123432"
            });
            await _context.SaveChangesAsync();
        }
    }
    
}