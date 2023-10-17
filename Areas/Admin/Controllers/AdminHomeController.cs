using Microsoft.AspNetCore.Mvc;
using static WebsiteBanHang.Data.ApplicaitonDbContext;
using WebsiteBanHang.Models;
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
        [Route("Index")]
        [Route("")]

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
    }
}
