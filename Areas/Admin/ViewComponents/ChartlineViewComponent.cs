using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Areas.Admin.Data;

namespace WebsiteBanHang.Areas.Admin.ViewComponents
{
    [Area("admin")]
    [Route("admin")]
    [Route("admin/homeadmin")]
    [Authorize(Roles = "Admin,Employee")]
    public class ChartlineViewComponent : ViewComponent
    {
        private readonly ILogger<ChartlineViewComponent> _logger;
        private readonly ApplicationDbContext _context;

        public ChartlineViewComponent(ApplicationDbContext context, ILogger<ChartlineViewComponent> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IViewComponentResult> InvokeAsync(int selectedYear)
        {
            try
            {
                // Query for total product quantity by status and month
                var productQuantityByStatus = await _context.Order
                    .Where(o => o.ngayBan.Year == selectedYear)
                    .GroupBy(o => new { o.trangThai, o.ngayBan.Month })
                    .Select(g => new { Status = g.Key.trangThai, Month = g.Key.Month, TotalQuantity = g.SelectMany(o => o.ctdh).Sum(d => d.soLuong) })
                    .ToListAsync();

                // Prepare data for the chart
                var statuses = productQuantityByStatus.Select(x => x.Status).Distinct().ToList();
                var months = Enumerable.Range(1, 12).ToList(); // Assuming all months from 1 to 12

                var chartData = statuses.Select(status => new
                {
                    Status = status,
                    Data = months.Select(month => productQuantityByStatus
                        .Where(x => x.Status == status && x.Month == month)
                        .Sum(x => x.TotalQuantity)).ToList()
                }).ToList();

                ViewBag.Months = months;
                ViewBag.ChartData = chartData;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while loading chart data.");
                return View("~/Areas/Admin/Views/Shared/_ErrorAdmin.cshtml");
            }
        }
    }

}
