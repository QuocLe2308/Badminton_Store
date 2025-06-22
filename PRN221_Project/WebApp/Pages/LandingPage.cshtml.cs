/*using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages
{
    public class LandingPageModel : PageModel
    {
        private readonly DataAccessLayer.DbContext.ApplicationDbContext _context;

        public LandingPageModel(DataAccessLayer.DbContext.ApplicationDbContext context)
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
}
*/
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Pages
{
	public class LandingPageModel : PageModel
	{
		private readonly DataAccessLayer.DbContext.ApplicationDbContext _context;

		public LandingPageModel(DataAccessLayer.DbContext.ApplicationDbContext context)
		{
			_context = context;
		}

		public IList<Product> NewProducts { get; set; } = default!;
		public IList<Product> SaleProducts { get; set; } = default!;
		public IList<Product> FeaturedProducts { get; set; } = default!;
		public IList<Product> Bestsellers { get; set; } = default!;

        public IList<Product> Category1Products { get; set; } = default!;
        public IList<Product> Category2Products { get; set; } = default!;
        public IList<Product> Category3Products { get; set; } = default!;
        public IList<Product> Category4Products { get; set; } = default!;
        public IList<Product> Category5Products { get; set; } = default!;
        public IList<Product> Category6Products { get; set; } = default!;
		public IList<Product> Category7Products { get; set; } = default!;

		public IList<Product> Brand10Products { get; set; } = default!;
        public IList<Product> Brand11Products { get; set; } = default!;
        public IList<Product> Brand12Products { get; set; } = default!;
        public IList<Product> Brand13Products { get; set; } = default!;
		public IList<Product> Brand14Products { get; set; } = default!;



		/// 
		/// LocShadow
		/// 
		public Dictionary<int, (string BrandName, IList<Product> Products)> BrandProducts { get; set; } = new Dictionary<int, (string BrandName, IList<Product> Products)>();
		public IList<Brand> Brands { get; set; } = new List<Brand>();


		private async Task<IList<Product>> GetByBrandAll(int brandID)
		{
			return await _context.Products
				.Include(p => p.Brand)
				.Include(p => p.Category)
				.Where(p => p.BrandID == brandID)
				.ToListAsync();
		}
		public Dictionary<int, (string CategoryName, IList<Product> Products)> CategoryProduct { get; set; } = new Dictionary<int, (string CategoryName, IList<Product> Products)>();
		public IList<Category> Categorys { get; set; } = new List<Category>();


		private async Task<IList<Product>> GetByCategorysAll(int brandID)
		{
			return await _context.Products
				.Include(p => p.Brand)
				.Include(p => p.Category)
				.Where(p => p.BrandID == brandID)
				.ToListAsync();
		}
		/// 
		/// LocShadow
		/// 



		public async Task OnGetAsync()
		{
			/// 
			/// LocShadow
			/// 
			Brands = await _context.Brands.ToListAsync();

			foreach (var brand in Brands)
			{
				var products = await GetByBrandAll(brand.BrandID);
				BrandProducts[brand.BrandID] = (brand.BrandName, products);
			}
			Categorys = await _context.Categories.ToListAsync();

			foreach (var category in Categorys)
			{
				var products = await GetByCategorysAll(category.CategoryID);
				CategoryProduct[category.CategoryID] = (category.CategoryName, products);
			}
			/// 
			/// LocShadow
			/// 
			NewProducts = await GetNewProductsAsync();
			SaleProducts = await GetSaleProductsAsync();

			FeaturedProducts = await GetFeaturedProductsAsync(NewProducts, SaleProducts);
			Bestsellers = await GetBestsellersAsync(NewProducts, SaleProducts);

            Category1Products = await GetByCategory1Async();
            Category2Products = await GetByCategory2Async();
            Category3Products = await GetByCategory3Async();
            Category4Products = await GetByCategory4Async();
            Category5Products = await GetByCategory5Async();
            Category6Products = await GetByCategory6Async();
			Category7Products = await GetByCategory7Async();

			Brand10Products = await GetByBrand10Async();
            Brand11Products = await GetByBrand11Async();
            Brand12Products = await GetByBrand12Async();
            Brand13Products = await GetByBrand13Async();
			Brand14Products = await GetByBrand14Async();
		}

		public async Task<IActionResult> OnGetNewProductsJsonAsync()
		{
			var products = await GetNewProductsAsync();
			return new JsonResult(products);
		}

		private async Task<IList<Product>> GetNewProductsAsync()
		{
			return await _context.Products
				.Include(p => p.Brand)
				.Include(p => p.Category)
				.OrderByDescending(p => p.ProductID) 
				.Take(5) 
				.ToListAsync();
		}

		private async Task<IList<Product>> GetSaleProductsAsync()
		{
			return await _context.Products
				.Include(p => p.Brand)
				.Include(p => p.Category)
				.OrderBy(p => p.ProductID) 
				.Take(5) 
				.ToListAsync();
		}

		private async Task<IList<Product>> GetFeaturedProductsAsync(IList<Product> newProducts, IList<Product> saleProducts)
		{
			var excludedIds = newProducts.Select(p => p.ProductID).Union(saleProducts.Select(p => p.ProductID)).ToList();
			return await _context.Products
				.Include(p => p.Brand)
				.Include(p => p.Category)
				.Where(p => p.ProductID % 2 == 0 && !excludedIds.Contains(p.ProductID))
				.ToListAsync();
		}

		private async Task<IList<Product>> GetBestsellersAsync(IList<Product> newProducts, IList<Product> saleProducts)
		{
			var excludedIds = newProducts.Select(p => p.ProductID).Union(saleProducts.Select(p => p.ProductID)).ToList();
			return await _context.Products
				.Include(p => p.Brand)
				.Include(p => p.Category)
				.Where(p => p.ProductID % 2 != 0 && !excludedIds.Contains(p.ProductID))
				.ToListAsync();
		}

        private async Task<IList<Product>> GetByCategory1Async()
        {
            return await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Where(p => p.CategoryID == 1)
                .ToListAsync();
        }

        private async Task<IList<Product>> GetByCategory2Async()
        {
            return await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Where(p => p.CategoryID == 2)
                .ToListAsync();
        }

        private async Task<IList<Product>> GetByCategory3Async()
        {
            return await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Where(p => p.CategoryID == 3)
                .ToListAsync();
        }

        private async Task<IList<Product>> GetByCategory4Async()
        {
            return await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Where(p => p.CategoryID == 4)
                .ToListAsync();
        }

        private async Task<IList<Product>> GetByCategory5Async()
        {
            return await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Where(p => p.CategoryID == 5)
                .ToListAsync();
        }

        private async Task<IList<Product>> GetByCategory6Async()
        {
            return await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Where(p => p.CategoryID == 6)
                .ToListAsync();
        }

		private async Task<IList<Product>> GetByCategory7Async()
		{
			return await _context.Products
				.Include(p => p.Brand)
				.Include(p => p.Category)
				.Where(p => p.CategoryID == 7)
				.ToListAsync();
		}

		private async Task<IList<Product>> GetByBrand10Async()
        {
            return await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Where(p => p.BrandID == 10)
                .ToListAsync();
        }

        private async Task<IList<Product>> GetByBrand11Async()
        {
            return await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Where(p => p.BrandID == 11)
                .ToListAsync();
        }

        private async Task<IList<Product>> GetByBrand12Async()
        {
            return await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Where(p => p.BrandID == 12)
                .ToListAsync();
        }

        private async Task<IList<Product>> GetByBrand13Async()
        {
            return await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Where(p => p.BrandID == 13)
                .ToListAsync();
        }

		private async Task<IList<Product>> GetByBrand14Async()
		{
			return await _context.Products
				.Include(p => p.Brand)
				.Include(p => p.Category)
				.Where(p => p.BrandID == 14)
				.ToListAsync();
		}
	}
}
