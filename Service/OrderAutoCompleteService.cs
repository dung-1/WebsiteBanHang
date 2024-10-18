using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebsiteBanHang.Areas.Admin.Data;
namespace WebsiteBanHang.Service
{
    public class OrderAutoCompleteService
    {
        private readonly IServiceProvider _serviceProvider;

        public OrderAutoCompleteService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Execute()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // Lọc các đơn hàng có trạng thái "Đang giao hàng" và thời gian giao hàng đã quá 2 phút
                var ordersToComplete = dbContext.Order
                    .Where(o => o.trangThai == "Đang giao hàng" &&
                                o.NgayGiaoHang.AddMinutes(2) <= DateTime.Now)
                    .ToList();

                foreach (var order in ordersToComplete)
                {
                    order.trangThai = "Hoàn thành";
                    // Thực hiện các thao tác khác nếu cần, như cập nhật kho hàng
                }

                dbContext.SaveChanges();
            }
        }
    }
}
