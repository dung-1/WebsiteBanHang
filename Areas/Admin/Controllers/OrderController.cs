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

            var productsQuery = from order in _context.Order
                                join userDetail in _context.Users_Details
                                on order.UserID equals userDetail.UserId into orderUserDetails
                                from userDetail in orderUserDetails.DefaultIfEmpty()
                                orderby order.id descending
                                select new
                                {
                                    Order = order,
                                    UserDetail = userDetail
                                };

            var sortedProducts = productsQuery.ToList();

            IPagedList<OderDto> pagedCategories = sortedProducts.Select(e => new OderDto
            {
                Id = e.Order.id,
                MaHoaDon = e.Order.MaHoaDon,
                TenNhanVien = e.UserDetail != null ? e.UserDetail.HoTen : null,
                NgayBan = e.Order.ngayBan,
                LoaiHoaDon = e.Order.LoaiHoaDon,
                TrangThai = e.Order.trangThai,
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

            // Tạo SelectList để sử dụng trong dropdown với giá trị mặc định "-"
            var userDetailSelectList = new List<SelectListItem>
                    {
                        new SelectListItem { Value = null, Text =null } // Mục mặc định
                    };


            userDetailSelectList.AddRange(userDetailList.Select(ud => new SelectListItem
            {
                Value = ud.UserId.ToString(),
                Text = ud.HoTen,
            }));

            ViewBag.UserDetailList = new SelectList(userDetailSelectList, "Value", "Text");

            return PartialView("_OrderEdit", order);
        }
        [HttpGet]
    //    public IActionResult Edit(int id)
    //    {
    //        var orders = (from order in _context.Order
    //                      join userDetail in _context.Users_Details
    //                      on order.UserID equals userDetail.UserId into orderUserDetails
    //                      from userDetail in orderUserDetails.DefaultIfEmpty()
    //                      where order.id == id
    //                      orderby order.id descending
    //                      select new
    //                      {
    //                          Order = order,
    //                          UserDetail = userDetail
    //                      })
    //                     .FirstOrDefault(); // Lấy một dòng đầu tiên hoặc null nếu không có

    //        // Lấy danh sách Users_Details
    //        var userDetailList = _context.Users_Details.ToList();

    //        // Tạo SelectList để sử dụng trong dropdown với giá trị mặc định "-"
    //        var userDetailSelectList = new List<SelectListItem>
    //{
    //    new SelectListItem { Value = null, Text = "Chọn người dùng" } // Mục mặc định
    //};

    //        userDetailSelectList.AddRange(userDetailList.Select(ud => new SelectListItem
    //        {
    //            Value = ud.UserId.ToString(),
    //            Text = ud.HoTen,
    //        }));

    //        ViewBag.UserDetailList = new SelectList(userDetailSelectList, "Value", "Text");

    //        return PartialView("_OrderEdit", orders);
    //    }




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
