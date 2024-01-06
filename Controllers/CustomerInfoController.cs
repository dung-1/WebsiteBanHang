using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebsiteBanHang.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerInfoController : Controller
    {
        public IActionResult AccountInfo()
        {
            return View();
        }
        public IActionResult Changepassword()
        {
            return View();
        }
        public IActionResult Setting()
        {
            return View();
        }

    }
}
