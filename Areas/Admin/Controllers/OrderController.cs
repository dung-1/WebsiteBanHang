using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteBanHang.Areas.Admin.AdminDTO;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Areas.Admin.Data;
using WebsiteBanHang.Areas.Admin.Models;
using X.PagedList;
using System.Linq;

namespace WebsiteBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly ILogger<OrderController> _logger;

        private readonly ApplicationDbContext _context;
        public OrderController(ApplicationDbContext context, ILogger<OrderController> logger)
        {
            _context = context;
            _logger = logger;
        }
        [Authorize(Roles = "Admin,Employee")]

        public IActionResult Index(int? page)
        {
            var pageNumber = page ?? 1; // Số trang mặc định (trang 1)
            int pageSize = 5; // Số mục trên mỗi trang


            var productsQuery = _context.Order
              .Include(p => p.user.userDetail)
              .OrderByDescending(p => p.id);

            var sortedProducts = productsQuery.ToList();


            IPagedList<OderDto> pagedCategories = sortedProducts.Select(e => new OderDto
            {
                Id = e.id,
                MaHoaDon = e.MaHoaDon,
                TenNhanVien = e.user.userDetail.HoTen,
                NgayBan = e.ngayBan,
                LoaiHoaDon = e.LoaiHoaDon,
                TrangThai = e.trangThai,
            }).ToPagedList(pageNumber, pageSize);
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }
            return View(pagedCategories);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var order = _context.Order.Include(o => o.user.userDetail).FirstOrDefault(o => o.id == id);

            if (order == null)
            {
                return NotFound();
            }

            // Lấy danh sách Users_Details
            var userDetailList = _context.Users_Details.ToList();

            // Tạo SelectList để sử dụng trong dropdown
            ViewBag.UserDetailList = new SelectList(userDetailList, "UserId", "HoTen");
            return PartialView("_OrderEdit", order);
        }


        public IActionResult Delete(int? id)
        {
            var deleterecord = _context.Product.Find(id);
            if (deleterecord == null)
            {
                return NotFound();
            }
            _context.Product.Remove(deleterecord);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
