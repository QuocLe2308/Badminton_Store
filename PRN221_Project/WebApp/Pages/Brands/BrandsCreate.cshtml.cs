using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DataAccessLayer.DbContext;
using DataAccessLayer.Models;

namespace WebApp.Pages.Brands
{
    public class BrandsCreate : PageModel
    {
        private readonly DataAccessLayer.DbContext.ApplicationDbContext _context;

        public BrandsCreate(DataAccessLayer.DbContext.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Brand Brand { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync(Brand brand)
        {
            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();

            return RedirectToPage("./BrandsViewList");
        }
    }
}
