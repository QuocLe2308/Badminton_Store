using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.Models;

namespace WebApp.Pages.Products
{
    public class ProductsViewList : PageModel
    {
        private readonly DataAccessLayer.DbContext.ApplicationDbContext _context;

        public ProductsViewList(DataAccessLayer.DbContext.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Product> Product { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category).ToListAsync();
        }
    }
}
