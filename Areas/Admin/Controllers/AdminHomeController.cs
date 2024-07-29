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
using OfficeOpenXml.Table;
using OfficeOpenXml;

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

                int currentMonth = DateTime.Now.Year == selectedYear ? DateTime.Now.Month : 12;

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

                // Prepare data for the revenue chart
                var revenueLabels = monthlyRevenueData.Select(x => $"{x.Month}/{x.Year}").ToList();
                var revenueData = monthlyRevenueData.Select(x => (decimal)x.TotalRevenue).ToList();

                // Query for product quantity by status
                var productQuantityByStatus = await _context.Order
                    .Where(o => o.ngayBan.Year == selectedYear)
                    .GroupBy(o => new { o.trangThai, o.ngayBan.Month })
                    .Select(g => new { Status = g.Key.trangThai, Month = g.Key.Month, InvoiceCount = g.Count() })
                    .ToListAsync();

                // Prepare data for the status chart
                var statuses = productQuantityByStatus.Select(x => x.Status).Distinct().ToList();
                var months = Enumerable.Range(1, currentMonth).Select(i => i.ToString()).ToList();

                var statusChartData = statuses.Select(status => new
                {
                    Status = status,
                    Data = months.Select(month => (decimal)productQuantityByStatus
                        .Where(x => x.Status == status && x.Month.ToString() == month)
                        .Sum(x => x.InvoiceCount)).ToList()
                }).Cast<dynamic>().ToList();

                // Query for additional statistics data (including TotalProductsSold)
                var additionalStatisticsData = await _context.Order
                    .Where(o => o.ngayBan.Year == selectedYear && o.trangThai == "Hoàn Thành")
                    .GroupBy(o => new { o.ngayBan.Year })
                    .Select(g => new StatisticsViewDto
                    {
                        TotalRevenue = (decimal)g.SelectMany(o => o.ctdh).Sum(d => d.gia),
                        TotalOrdersCount = g.Count(),
                        TotalProductsSold = g.SelectMany(o => o.ctdh).Sum(d => d.soLuong)
                    })
                    .ToListAsync();

                // Query for inventory category percentages
                var totalInventory = await _context.Inventory.SumAsync(i => i.SoLuong);
                var inventoryCategoryPercentages = await _context.Inventory
                    .GroupBy(i => i.product.Category.TenLoai)
                    .Select(g => new InventoryCategoryPercentageDto
                    {
                        CategoryName = g.Key,
                        Percentage = (decimal)g.Sum(i => i.SoLuong) / totalInventory * 100
                    })
                    .ToListAsync();

                // Query for product inventories
                var productInventories = await _context.Inventory
                    .GroupBy(i => i.product.TenSanPham)
                    .Select(g => new ProductInventoryDto
                    {
                        ProductName = g.Key,
                        Quantity = g.Sum(i => i.SoLuong)
                    })
                    .ToListAsync();

                // Create the view model
                var model = new DashboardViewModel
                {
                    Statistics = new StatisticsViewDto
                    {
                        TotalRevenue = additionalStatisticsData.Sum(x => x.TotalRevenue),
                        TotalOrdersCount = additionalStatisticsData.Sum(x => x.TotalOrdersCount),
                        TotalProductsSold = additionalStatisticsData.Sum(x => x.TotalProductsSold)
                    },
                    RevenueLabels = revenueLabels,
                    RevenueData = revenueData,
                    Months = months,
                    ChartData = statusChartData,
                    InventoryCategoryPercentages = inventoryCategoryPercentages,
                    ProductInventories = productInventories
                };

                return View(model);
            }
            catch (Exception ex)
            {
                return View("~/Areas/Admin/Views/Shared/_ErrorAdmin.cshtml");
            }
        }


        public byte[] ExporttoExcel<T>(List<T> table, string filename)
        {
            using ExcelPackage pack = new ExcelPackage();
            ExcelWorksheet ws = pack.Workbook.Worksheets.Add(filename);
            ws.Cells["A1"].LoadFromCollection(table, true, TableStyles.Light1);
            return pack.GetAsByteArray();
        }
    }
}
