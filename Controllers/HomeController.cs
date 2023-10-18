using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using WebsiteBanHang.Areas.Admin.Data;
using WebsiteBanHang.Areas.Admin.Models;
using WebsiteBanHang.Models;
using static WebsiteBanHang.Areas.Admin.Data.ApplicationDbContext;
using X.PagedList;
namespace WebsiteBanHang.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomeController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index(int? page)
        {
            var pageNumber = page ?? 1; // Số trang mặc định (trang 1)
            int pageSize = 8; // Số mục trên mỗi trang

            // Lấy danh sách sản phẩm từ cơ sở dữ liệu (điều này cần phải được thực hiện theo cách bạn đã thực hiện ở controller)
            List<ProductModel> products = _context.Product.ToList();

            // Sử dụng PagedList để phân trang
            IPagedList<ProductModel> pagedProducts = products.ToPagedList(pageNumber, pageSize);

            return View(pagedProducts);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}