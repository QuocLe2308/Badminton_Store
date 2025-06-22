using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.DbContext;
using DataAccessLayer.Models;

namespace WebApp.Pages.Products
{
    public class DetailsModel : PageModel
    {
        private readonly DataAccessLayer.DbContext.ApplicationDbContext _context;

        public DetailsModel(DataAccessLayer.DbContext.ApplicationDbContext context)
        {
            _context = context;
        }

        public Product Product { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category) // Nạp sẵn thực thể Category
                .Include(p => p.Brand) // Nạp sẵn thực thể Brand
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }
            else
            {
                Product = product;
            }
            return Page();
        }
    }
}
