using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using DataAccessLayer.DbContext;
using DataAccessLayer.Models;

namespace WebApp.Pages.Products
{
    public class ProductsCreate : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ImageUploadService _imageUploadService;

        public ProductsCreate(ApplicationDbContext context, ImageUploadService imageUploadService)
        {
            _context = context;
            _imageUploadService = imageUploadService;
        }

        public IActionResult OnGet()
        {
            ViewData["BrandID"] = new SelectList(_context.Brands, "BrandID", "BrandName");
            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName");
            return Page();
        }

        [BindProperty]
        public Product Product { get; set; } = default!;

        [BindProperty]
        public IFormFile File { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {


            if (File != null && File.Length > 0)
            {
                try
                {
                    var fileName = await _imageUploadService.UploadImageAsync(File);
                    var fileUrl = Url.Content($"~/uploads/{fileName}");
                    Product.ImageURL = fileUrl;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Internal server error: {ex.Message}");
                    return Page();
                }
            }
            
            _context.Products.Add(Product);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Products/ProductsViewList");

        }
    }
}
