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
        private static string AdminConnectionId { get; set; }

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

            // Lưu ConnectionId vào UserModel
            var user = await _context.User.SingleOrDefaultAsync(c => c.Id == int.Parse(userId));
            if (user != null)
            {
                user.ChatConnectionId = Context.ConnectionId;
                await _context.SaveChangesAsync();
            }
            var users = Context.User; // Get the user from the context

            if (users.IsInRole("Admin"))
            {
                AdminConnectionId = Context.ConnectionId;
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

            await base.OnDisconnectedAsync(exception);
            await SendCustomerListToAdmin();


        }
        private async Task<int?> GetCustomerIdFromConnectionId(string connectionId)
        {
            var chatConnection = await _context.ChatConnection
                .Include(cc => cc.User)
                .FirstOrDefaultAsync(cc => cc.ConnectionId == connectionId);

            if (chatConnection == null)
            {
                return null;
            }

            if (chatConnection.User is CustomerModel customer)
            {
                return customer.Id;
            }

            return null;
        }

        public async Task SendMessageToAdmin(string message)
        {
            try
            {
                var senderConnectionId = Context.ConnectionId;

                // Lấy CustomerId từ ConnectionId
                var customerId = await GetCustomerIdFromConnectionId(senderConnectionId);
                if (customerId == null)
                {
                    throw new Exception("Không tìm thấy CustomerId cho kết nối này.");
                }

                var adminUser = await _context.User
                    .Include(u => u.ChatConnection) // eager load ChatConnection
                    .FirstOrDefaultAsync(u => u.UserRole.Any(r => r.Role.Name == "Admin")); // filter by admin role

                if (adminUser == null)
                {
                    throw new Exception("Không tìm thấy admin.");
                }

                var chatMessage = new ChatMessage
                {
                    Content = message,
                    SentAt = DateTime.Now,
                    ConnectionIdFrom = senderConnectionId,
                    ConnectionIdTo = adminUser.ChatConnection.ConnectionId
                };

                _context.ChatMessage.Add(chatMessage);
                await _context.SaveChangesAsync();

                await Clients.Client(adminUser.ChatConnection.ConnectionId).SendAsync("ReceiveMessages", customerId, message, chatMessage.SentAt);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in SendMessageToAdmin: {ex.Message}");
                throw;
            }
        }


        public async Task SendMessageToCustomer(int customerId, string message)
        {
            try
            {
                var senderConnectionId = Context.ConnectionId;

                // Logging customerId to ensure it's being received correctly
                Console.WriteLine($"SendMessageToCustomer called with customerId: {customerId}");

                var customer = await _context.Customer
                    .Include(u => u.ChatConnection) // eager load ChatConnection
                    .FirstOrDefaultAsync(u => u.Id == customerId);

                var chatMessage = new ChatMessage
                {
                    Content = message,
                    SentAt = DateTime.Now,
                    ConnectionIdFrom = senderConnectionId,
                    ConnectionIdTo = customer.ChatConnection.ConnectionId
                };

                _context.ChatMessage.Add(chatMessage);
                await _context.SaveChangesAsync();
                //đây là đăng gửi cho tất cả vì dùng clients.All chứ nếu nhắn 1-1 dùng clients.client()
                await Clients.All.SendAsync("ReceiveMessagetoCustomer", senderConnectionId, message, chatMessage.SentAt);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in SendMessageToCustomer: {ex.Message}");
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
                    .Where(m => connectionIds.Contains(m.ConnectionIdFrom) || connectionIds.Contains(m.ConnectionIdTo))
                    .OrderBy(m => m.SentAt)
                    .Select(m => new ChatMessageModel
                    {
                        SenderId = m.ConnectionIdFrom,
                        Message = m.Content,
                        SentAt = m.SentAt,
                        IsFromAdmin = connectionIds.All(id => m.ConnectionIdFrom != id) // Check if sender is not in customer's connections
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




        public async Task<List<CustomerViewModel>> GetCustomerList()

        {
            var customers = await _context.ChatConnection
                .Where(cc => cc.User is CustomerModel) // Lọc các kết nối là khách hàng
                .GroupBy(cc => cc.UserId) // Nhóm theo UserId để chỉ lấy một kết nối duy nhất cho mỗi khách hàng
                .Select(group => new CustomerViewModel
                {
                    Id = group.Key,
                    Name = group.First().User.Email, // Lấy tên từ Customer_Details
                    LastMessage = group.SelectMany(cc => _context.ChatMessage
                        .Where(cm => cm.ConnectionIdTo == cc.ConnectionId || cm.ConnectionIdFrom == cc.ConnectionId))
                        .OrderByDescending(cm => cm.SentAt)
                        .Select(cm => cm.Content)
                        .FirstOrDefault() ?? "No messages", // Lấy tin nhắn cuối cùng
                    LastMessageTimeAgo = group.SelectMany(cc => _context.ChatMessage
                        .Where(cm => cm.ConnectionIdTo == cc.ConnectionId || cm.ConnectionIdFrom == cc.ConnectionId))
                        .OrderByDescending(cm => cm.SentAt)
                        .Select(cm => cm.SentAt) // Trả về DateTime của tin nhắn cuối cùng
                        .FirstOrDefault()
                })
                .ToListAsync();

            return customers;
        }

        public async Task<List<ChatMessageViewModel>> GetMessages(int customerId)
        {
            try
            {
                // Lấy userId của admin
                int adminUserId = 8; // Đây là ví dụ, bạn cần thay thế bằng cách lấy userId của admin trong hệ thống của bạn

                var messages = await _context.ChatMessage
                    .Where(m =>
                        (m.FromConnection.UserId == customerId && m.ToConnection.UserId == adminUserId) || // Tin nhắn từ khách hàng tới admin
                        (m.FromConnection.UserId == adminUserId && m.ToConnection.UserId == customerId)   // Tin nhắn từ admin tới khách hàng
                    )
                    .OrderBy(m => m.SentAt)
                    .Select(m => new ChatMessageViewModel
                    {
                        Content = m.Content,
                        SentAt = m.SentAt,
                        SenderId = m.ConnectionIdFrom, // Id người gửi tin nhắn
                        IsAdminMessage = m.FromConnection.UserId == adminUserId // Đánh dấu tin nhắn của admin
                    })
                    .ToListAsync();

                return messages;
            }
            catch (Exception ex)
            {
                // Ghi log lỗi
                Console.WriteLine($"Error in GetMessages: {ex.Message}");
                throw;
            }
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

