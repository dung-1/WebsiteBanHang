using Microsoft.AspNetCore.Mvc;
using WebsiteBanHang.Areas.Admin.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebsiteBanHang.Areas.Admin.AdminDTO;
using WebsiteBanHang.Areas.Admin.Models;

namespace WebsiteBanHang.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin")]
    [Route("admin/homeadmin")]
    [Authorize(Roles = "Admin,Employee")]
    public class AdminHomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminHomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? selectedYear)

        {

            try
            {
                if (!selectedYear.HasValue)
                {
                    selectedYear = DateTime.Now.Year;
                }

                // Query for monthly revenue data
                IQueryable<OrdersModel> query = _context.Order.Where(o => o.ngayBan.Year == selectedYear && o.trangThai == "Hoàn Thành");

                var monthlyRevenueData = await query
                    .GroupBy(o => new { o.ngayBan.Year, o.ngayBan.Month })
                    .Select(g => new { Year = g.Key.Year, Month = g.Key.Month, TotalRevenue = g.Sum(o => o.tongTien) })
                    .OrderBy(x => x.Year)
                    .ThenBy(x => x.Month)
                    .ToListAsync();

                // Populate combobox with available years
                var availableYears = await _context.Order
                    .Where(x => x.trangThai == "Hoàn Thành")
                    .Select(o => o.ngayBan.Year)
                    .Distinct()
                    .OrderByDescending(year => year)
                    .ToListAsync();

                ViewBag.AvailableYears = availableYears;
                ViewBag.SelectedYear = selectedYear;

                // Prepare data for chart
                var labels = monthlyRevenueData.Select(x => $"{x.Month}/{x.Year}").ToList();
                var data = monthlyRevenueData.Select(x => x.TotalRevenue).ToList();

                ViewBag.Labels = labels;
                ViewBag.Data = data;

                // Query for additional statistics data (including TotalProductsSold)
                var additionalStatisticsData =
                    await _context.Order
                        .Where(o => o.ngayBan.Year == selectedYear && o.trangThai == "Hoàn Thành")
                        .GroupBy(o => new { o.ngayBan.Year })
                        .Select(g => new StatisticsViewDto
                        {
                            TotalRevenue = (decimal)g.SelectMany(o => o.ctdh).Sum(d => d.gia),
                            TotalOrdersCount = g.Select(o => o.MaHoaDon).Distinct().Count(),
                            TotalProductsSold = g.SelectMany(o => o.ctdh).Sum(d => d.soLuong)
                        })
                        .ToListAsync();

                // Create the view model
                var model = new StatisticsViewDto
                {
                    TotalRevenue = additionalStatisticsData.Sum(x => x.TotalRevenue),
                    TotalOrdersCount = additionalStatisticsData.Sum(x => x.TotalOrdersCount),
                    TotalProductsSold = additionalStatisticsData.Sum(x => x.TotalProductsSold)
                };

                return View(model);
            }
            catch (Exception ex)
            {
                return View("~/Areas/Admin/Views/Shared/_ErrorAdmin.cshtml");

            }

        }
    }
}
