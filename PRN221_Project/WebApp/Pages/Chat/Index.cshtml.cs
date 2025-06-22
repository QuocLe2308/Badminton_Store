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
    public class Chat : PageModel
    {
        private readonly ILogger<Chat> _logger;
		private readonly ApplicationDbContext _context;


		public Chat(ILogger<Chat> logger, ApplicationDbContext context)
        {
            _logger = logger;
			_context = context;
		}

		public IList<Messages> Messages { get; set; }
        

        public async Task OnGetAsync()
		{
            //Console.WriteLine(HttpContext.Session.GetInt32("UserId")); 
            //Messages = await _context.Messages.Where(m => m.CustomerID == HttpContext.Session.GetInt32("UserId")).Include(m => m.User).ToListAsync();
            Messages = await _context.Messages
    .Include(m => m.User)
    .OrderByDescending(m => m.TimeSend)
    .Take(10)
    .OrderBy(m => m.TimeSend) // Order back to ascending to show the messages in the correct order
    .ToListAsync();

        }
    }
}