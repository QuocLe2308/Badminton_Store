using System;
using System.Threading.Tasks;
using DataAccessLayer.DbContext;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Hubs
{
    public class ChatHubs : Hub
    {
        private readonly ApplicationDbContext _context;

        public ChatHubs(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SendMessage(string user, string message)
        {
            var chatMessage = new Messages
            {
                MessageSend = message,
                TimeSend = DateTime.UtcNow,
                Status = "Sent",
                CustomerID = GetCustomerIdFromContext()
            };

            _context.Messages.Add(chatMessage);
            await _context.SaveChangesAsync();
            User users = await _context.Users.FirstOrDefaultAsync(u => u.CustomerID == GetCustomerIdFromContext());
            string nameshow;
            if (users.Role == "Admin")
            {
                nameshow = " ( Admin )";
            }
            else
            {
                nameshow = "";
            }
            await Clients.All.SendAsync("ReceiveMessage", users.Username+ nameshow, message, chatMessage.TimeSend);
        }

        private int GetCustomerIdFromContext()
        {
            var customerId = Context.GetHttpContext().Session.GetInt32("UserId");
            return customerId ?? 0;
        }
    }
}
