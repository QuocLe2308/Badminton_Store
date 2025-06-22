using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages;

public class Dashboard : PageModel
{
    private readonly DataAccessLayer.DbContext.ApplicationDbContext _context;

    public Dashboard(DataAccessLayer.DbContext.ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<Product> Product { get; set; } = default!;

    public async Task OnGetAsync()
    {
        Product = await _context.Products
            .Include(p => p.Brand)
            .Include(p => p.Category).ToListAsync();
    }
}
