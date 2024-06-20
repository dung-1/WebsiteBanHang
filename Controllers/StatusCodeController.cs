using Microsoft.AspNetCore.Mvc;

namespace WebsiteBanHang.Controllers
{
    public class StatusCodeController : Controller
    {
        [Route("StatusCode/{code}")]
        public IActionResult Index(int code)
        {
            switch (code)
            {
                case 404:
                    return View("404");
                case 500:
                    return View("500");
                default:
                    return View("Error");
            }
        }
    }
}
