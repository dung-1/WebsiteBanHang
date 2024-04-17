using Microsoft.AspNetCore.SignalR;
using static i18n.Helpers.NuggetParser;
using WebsiteBanHang.Areas.Admin.Data;
using WebsiteBanHang.Areas.Admin.Models;

namespace WebsiteBanHang.HubSignalR
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;

        public ChatHub(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SendMessage(string message, string recipientConnectionId)
        {
            // 1. Save the message to the database
            var chatMessage = new ChatMessage
            {
                Content = message,
                SentAt = DateTime.Now,
                ConnectionIdFrom = Context.ConnectionId, // Sender's connection ID
                ConnectionIdTo = recipientConnectionId
            };

            _context.Add(chatMessage);
            await _context.SaveChangesAsync();

            // 2. Send the message to the recipient client
            await Clients.Client(recipientConnectionId).SendAsync("ReceiveMessage", message);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // 3. Handle user disconnection (optional)
            // You can update the user's online status or perform other actions here
            await base.OnDisconnectedAsync(exception);
        }

        // ... other methods for connection management, user lookup, etc.
    }

}
