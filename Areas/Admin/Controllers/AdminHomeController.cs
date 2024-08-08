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
using WebsiteBanHang.Areas.Admin.Common;

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
                IQueryable<OrdersModel> query = _context.Order.Where(o => o.ngayBan.Year == selectedYear && o.trangThai == "Hoàn thành");

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


                // Query for product quantity by status
                var productQuantityByStatus = await _context.Order
                    .Where(o => o.ngayBan.Year == selectedYear)
                    .GroupBy(o => new { o.trangThai, o.ngayBan.Month })
                    .Select(g => new { Status = g.Key.trangThai, Month = g.Key.Month, InvoiceCount = g.Count() })
                    .ToListAsync();


                // Prepare data for the revenue chart
                var revenueLabels = monthlyRevenueData.Select(x => $"{x.Month}/{x.Year}").ToList();
                var revenueData = monthlyRevenueData.Select(x => (decimal)x.TotalRevenue).ToList();

                var statuses = productQuantityByStatus.Select(x => x.Status).Distinct().ToList();
                var months = Enumerable.Range(1, currentMonth).Select(i => i.ToString()).ToList();

                var statusChartData = statuses.Select(status => new
                {
                    Status = status,
                    Data = months.Select(month => (decimal)productQuantityByStatus
                        .Where(x => x.Status == status && x.Month.ToString() == month)
                        .Sum(x => x.InvoiceCount)).ToList()
                }).Cast<dynamic>().ToList();

                // Query for product inventories grouped by brand and product
                var productInventories = await _context.Inventory
                    .GroupBy(i => new { i.product.Brand.TenHang, i.product.TenSanPham })
                    .Select(g => new
                    {
                        BrandName = g.Key.TenHang,
                        ProductName = g.Key.TenSanPham,
                        Quantity = g.Sum(i => i.SoLuong)
                    })
                    .GroupBy(x => x.BrandName)
                    .Select(g => new BrandInventoryDto
                    {
                        BrandName = g.Key,
                        ProductDetails = g.Select(p => new ProductDetailDto
                        {
                            ProductName = p.ProductName,
                            Quantity = p.Quantity
                        }).ToList()
                    })
                    .ToListAsync();


                // Truy vấn để lấy dữ liệu thống kê số lượng bài viết theo category và tổng số lượt xem
                var categoryStats = await _context.CategoryPost
                    .Select(c => new
                    {
                        CategoryName = c.Name,
                        PostCount = c.Posts.Count(p => p.CreatedTime.Year == selectedYear),
                        TotalViewCount = c.Posts.Where(p => p.CreatedTime.Year == selectedYear).Sum(p => p.ViewCount)
                    })
                    .ToListAsync();

                // Chuẩn bị dữ liệu cho biểu đồ
                var categoryLabels = categoryStats.Select(cs => cs.CategoryName).ToList();
                var postCounts = categoryStats.Select(cs => cs.PostCount).ToList();

                // Create the view model
                var model = new DashboardViewModel
                {
                    CategoryLabels = categoryLabels,
                    PostCounts = postCounts,
                    RevenueLabels = revenueLabels,
                    RevenueData = revenueData,
                    Months = months,
                    ChartData = statusChartData,
                    BrandInventories = productInventories // Gán giá trị cho BrandInventories
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
