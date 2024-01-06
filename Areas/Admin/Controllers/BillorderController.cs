using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Areas.Admin.Data;
using WebsiteBanHang.Areas.Admin.Models;

namespace WebsiteBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Employee")]
    public class BillorderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Approved()
        {
            return View();
        }
        public IActionResult Transport()
        {
            return View();
        }
        public IActionResult Complete()
        {
            return View();
        }
    }
}
