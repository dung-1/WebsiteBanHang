using Microsoft.AspNetCore.Mvc;


namespace WebsiteBanHang.Areas.Admin.Controllers
{
    public class FileManagerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
