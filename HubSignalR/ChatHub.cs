using Microsoft.AspNetCore.SignalR;
using WebsiteBanHang.Areas.Admin.Data;
using WebsiteBanHang.Areas.Admin.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebsiteBanHang.Models;
using Microsoft.Extensions.DependencyInjection;
using static i18n.Helpers.NuggetParser;

namespace WebsiteBanHang.HubSignalR
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;

        public ChatHub(ApplicationDbContext context)
        {
            _context = context;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User.FindFirst("UserId").Value;

            var connection = new ChatConnection
            {
                ConnectionId = Context.ConnectionId,
                UserId = int.Parse(userId),
                Connected = true,
                LastActive = DateTime.Now
            };

            _context.ChatConnection.Add(connection);
            await _context.SaveChangesAsync();

            // Lưu ConnectionId vào CustomerModel
            var customer = await _context.Customer.SingleOrDefaultAsync(c => c.Id == int.Parse(userId));
            if (customer != null)
            {
                customer.ChatConnectionId = Context.ConnectionId;
                await _context.SaveChangesAsync();
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            await base.OnConnectedAsync();
            await SendCustomerListToAdmin();
        }


        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var connection = _context.ChatConnection
                .FirstOrDefault(c => c.ConnectionId == Context.ConnectionId);

            if (connection != null)
            {
                connection.Connected = false;
                connection.LastActive = DateTime.Now;

                _context.ChatConnection.Update(connection);
                await _context.SaveChangesAsync();
            }

            await SendCustomerListToAdmin();
            await base.OnDisconnectedAsync(exception);

        }

        public async Task SendMessageToAdmin(string adminConnectionId, string message)
        {
            try
            {
                var senderConnectionId = Context.ConnectionId;

                var adminUser = await _context.User
                      .Include(u => u.ChatConnection) // eager load ChatConnection
                      .FirstOrDefaultAsync(u => u.UserRole.Any(r => r.Role.Name == "Admin")); // filter by admin role
                if (adminUser == null)
                {
                    throw new Exception("Không tìm thấy khách hàng.");
                }

                var chatMessage = new ChatMessage
                {
                    Content = message,
                    SentAt = DateTime.Now,
                    ConnectionIdFrom = senderConnectionId,
                    ConnectionIdTo = adminUser.ChatConnectionId
                };


                _context.ChatMessage.Add(chatMessage);
                await _context.SaveChangesAsync();

                await Clients.Client(adminConnectionId).SendAsync("ReceiveMessage", senderConnectionId, message);

            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in SendMessageToAdmin: {ex.Message}");
                throw;
            }
        }
        public async Task<List<ChatMessageModel>> GetChatHistory(int userId)
            {
            try
            {
                // Lấy danh sách các ConnectionId liên quan đến userId
                var connectionIds = await _context.ChatConnection
                    .Where(cc => cc.UserId == userId)
                    .Select(cc => cc.ConnectionId)
                    .ToListAsync();

                if (connectionIds == null || connectionIds.Count == 0)
                {
                    throw new Exception("User connections not found.");
                }

                var messages = await _context.ChatMessage
                    .Where(m => connectionIds.Contains(m.ConnectionIdFrom)) // Lọc tin nhắn tới các ConnectionId của người dùng
                    .OrderBy(m => m.SentAt)
                    .Select(m => new ChatMessageModel
                    {
                        SenderId = m.ConnectionIdFrom,
                        Message = m.Content,
                          SentAt = m.SentAt
                    })
                    .ToListAsync();

                return messages;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetChatHistory: {ex.Message}");
                throw;
            }
        }


        private async Task<List<CustomerViewModel>> GetCustomerList()
        {
            var customers = await _context.ChatConnection
                .Where(cc => cc.User is CustomerModel) // Lọc các kết nối là khách hàng
                .Select(cc => new CustomerViewModel
                {
                    Id = cc.UserId,
                    Name = (cc.User as CustomerModel).CustomerDetail.HoTen, // Giả sử HoTen là thuộc tính tên trong CustomerDetail
                    LastMessage = _context.ChatMessage
                        .Where(cm => cm.ConnectionIdTo == cc.ConnectionId || cm.ConnectionIdFrom == cc.ConnectionId)
                        .OrderByDescending(cm => cm.SentAt)
                        .Select(cm => cm.Content)
                        .FirstOrDefault() ?? "No messages", // Nếu không tìm thấy tin nhắn
                    LastMessageTimeAgo = _context.ChatMessage
                        .Where(cm => cm.ConnectionIdTo == cc.ConnectionId || cm.ConnectionIdFrom == cc.ConnectionId)
                        .OrderByDescending(cm => cm.SentAt)
                        .Select(cm => cm.SentAt) // Trả về DateTime của tin nhắn cuối cùng
                        .FirstOrDefault()
                })
                .ToListAsync();

            return customers;
        }


        private async Task SendCustomerListToAdmin()
        {
            try
            {
                var customers = await GetCustomerList();
                await Clients.All.SendAsync("UpdateCustomerList", customers);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending customer list to admin: {ex.Message}");
            }
        }

    }
}