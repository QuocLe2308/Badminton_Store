using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.DbContext;
using DataAccessLayer.Models;

namespace WebApp.Pages.Orders
{
    public class OrdersViewList : PageModel
    {
        private readonly ApplicationDbContext _context;

        public OrdersViewList(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Order> Order { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (HttpContext.Session.GetString("Role") == "Admin")
            {
                Order = await _context.Orders
                    .Include(o => o.User)
                    .ToListAsync();
            }
            else
            {
                int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
                Order = await _context.Orders
                    .Include(o => o.User)
                    .Where(o => o.CustomerID == userId)
                    .ToListAsync();
            }

        }
    }
}
