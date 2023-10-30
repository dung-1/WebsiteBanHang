using Microsoft.AspNetCore.Mvc;
using static WebsiteBanHang.Areas.Admin.Data.ApplicationDbContext;
using WebsiteBanHang.Models;
using WebsiteBanHang.Areas.Admin.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;

namespace WebsiteBanHang.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin")]
    [Route("admin/homeadmin")]
    public class AdminHomeController : Controller

    {
        private readonly ApplicationDbContext _context;
        public AdminHomeController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Route("admin")]
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            return View();
        }
        [Route("Order")]
        public IActionResult Order()
        {
            return View();
        }
        [Route("OrderDetail")] 
        public IActionResult OrderDetail()
        {
            return View();
        }
        public IActionResult Logout()
        {
            // Đăng xuất người dùng bằng cách xóa phiên đăng nhập
            HttpContext.SignOutAsync();

            // Chuyển đến trang đăng nhập trong controller Account bên ngoài khu vực Admin
            return RedirectToAction("Login", "Account", new { area = "" });
        }

    }
}
