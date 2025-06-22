using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.DbContext;
using DataAccessLayer.Models;

namespace WebApp.Pages.OrderDetails
{
    public class IndexModel : PageModel
    {
        private readonly DataAccessLayer.DbContext.ApplicationDbContext _context;

        public IndexModel(DataAccessLayer.DbContext.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<OrderDetail> OrderDetail { get;set; } = default!;

        public async Task OnGetAsync()
        {
            OrderDetail = await _context.OrderDetails
                .Include(o => o.Order)
                .Include(o => o.Product).ToListAsync();
        }
    }
}
