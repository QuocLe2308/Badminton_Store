using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages
{
    public class SingleProductModel : PageModel
    {
		private readonly DataAccessLayer.DbContext.ApplicationDbContext _context;

		public SingleProductModel(DataAccessLayer.DbContext.ApplicationDbContext context)
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
				.Include(p => p.Category) // N?p s?n th?c th? Category
				.Include(p => p.Brand) // N?p s?n th?c th? Brand
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
