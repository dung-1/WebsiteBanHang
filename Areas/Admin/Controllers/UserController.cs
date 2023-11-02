using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Areas.Admin.AdminDTO;
using WebsiteBanHang.Areas.Admin.Data;
using WebsiteBanHang.Areas.Admin.Models;
using X.PagedList;
using static WebsiteBanHang.Areas.Admin.Data.ApplicationDbContext;

namespace WebsiteBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;

        private readonly ApplicationDbContext _context;
        public UserController(ApplicationDbContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;

        }
        public IActionResult Index(int? page, string searchName)
        {
            var pageNumber = page ?? 1;
            int pageSize = 5;

            var productsQuery = _context.User
                .Include(p => p.UserRole)
                .Include(p => p.userDetail)
                .OrderByDescending(p => p.Id);

            if (!string.IsNullOrEmpty(searchName))
            {
                productsQuery = (IOrderedQueryable<UserModel>)productsQuery.Where(p => p.userDetail.HoTen.Contains(searchName));
            }

            var sortedProducts = productsQuery.ToList();

            if (searchName != null)
            {
                ViewBag.SearchName = searchName;
            }
            else
            {
                ViewBag.SearchName = ""; // Hoặc gán một giá trị mặc định khác nếu cần thiết
            }

            IPagedList<UserModelViewDto> pagedProducts = sortedProducts
                .Select(e => new UserModelViewDto
                {
                    Id = e.Id,
                    MaNguoiDung = e.MaNguoiDung,
                    HoTen = e.userDetail.HoTen,
                    Email = e.Email,
                    SoDienThoai = e.userDetail.SoDienThoai,
                    //VaiTro=e.UserRole.


                })
                .ToPagedList(pageNumber, pageSize);

            return View(pagedProducts);
        }
    }
}
