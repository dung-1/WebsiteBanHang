using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Areas.Admin.Data;
using WebsiteBanHang.Areas.Admin.Models;
using WebsiteBanHang.Areas.Admin.AdminDTO;

using X.PagedList;

namespace WebsiteBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderDetailController : Controller
    {
        private readonly ILogger<OrderDetailController> _logger;

        private readonly ApplicationDbContext _context;
        public OrderDetailController(ApplicationDbContext context, ILogger<OrderDetailController> logger)
        {
            _context = context;
            _logger = logger;
        }
        [Authorize(Roles = "Admin,Employee")]
        public IActionResult Index(int? page)
        {
            try
            {
                var pageNumber = page ?? 1; // Số trang mặc định (trang 1)
                int pageSize = 5; // Số mục trên mỗi trang

                // Lấy dữ liệu sản phẩm đã sắp xếp
                var productsQuery = _context.Order_Detai
                    .Include(p => p.product)
                    .Include(p => p.order)
                    .OrderByDescending(p => p.ID);
                var sortedProducts = productsQuery.ToList();

                // Tạo danh sách sản phẩm đã sắp xếp dưới dạng danh sách DTO
                var pagedCategories = sortedProducts.Select(e => new OderDetailDto
                {
                    Id = e.ID,
                    MaHoaDon = e.order.MaHoaDon,
                    TenSanPham = e.product.TenSanPham,
                    SoLuong = e.soLuong,
                    Gia = e.gia,
                }).ToPagedList(pageNumber, pageSize); // Sử dụng PagedList.Mvc để phân trang

                if (TempData["SuccessMessage"] != null)
                {
                    ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
                }

                return View(pagedCategories);
            }
            catch (Exception ex)
            {
                return View("~/Areas/Admin/Views/Shared/_ErrorAdmin.cshtml");
            }

        }
        public IActionResult Delete(int? id)
        {
            try
            {
                var deleterecord = _context.Product.Find(id);
                if (deleterecord == null)
                {
                    return View("~/Areas/Admin/Views/Shared/_ErrorAdmin.cshtml");
                }
                _context.Product.Remove(deleterecord);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View("~/Areas/Admin/Views/Shared/_ErrorAdmin.cshtml");
            }

        }

    }
}
