using Microsoft.AspNetCore.SignalR;

namespace WebsiteBanHang.HubSignalR
{
    public class NotificationHub : Hub
    {
        public async Task SendOrderNotification(string orderCode)
        {
            await Clients.All.SendAsync("ReceiveOrderNotification", orderCode);
        }
    }
}
