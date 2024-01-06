using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebsiteBanHang.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerOrderController : Controller
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
        public IActionResult OrderDetail()
        {
            return View();
        }
    }
}
