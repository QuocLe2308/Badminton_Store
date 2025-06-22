using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer.DbContext;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Chat
{
    public class ReplyModel : PageModel
    {
        private readonly ILogger<Chat> _logger;
        private readonly ApplicationDbContext _context;


        public ReplyModel(ILogger<Chat> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IList<Messages> Messages { get; set; }


        public async Task OnGetAsync()
        {
            Console.WriteLine(HttpContext.Session.GetInt32("UserId"));
            Messages = await _context.Messages.Where(m => m.CustomerID == 2).Include(m => m.User).ToListAsync();
        }
    }
}