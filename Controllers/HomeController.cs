using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebsiteBanHang.Areas.Admin.Data;
using WebsiteBanHang.Areas.Admin.Models;
using WebsiteBanHang.Models;
using X.PagedList;
using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Areas.Admin.AdminDTO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Localization;

namespace WebsiteBanHang.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer<HomeController> _localizer;

        public HomeController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IStringLocalizer<HomeController> localizer)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _localizer = localizer;
        }
        public async Task<IActionResult> Index(int? page, string searchName, string selectedCategory)
        {
            var pageNumber = page ?? 1;
            int pageSize = 12;

            var products = _context.Product
                   .Select(p => new ProductModel
                   {
                       Id = p.Id,
                       MaSanPham = p.MaSanPham,
                       TenSanPham = p.TenSanPham,
                       HangId = p.HangId,
                       Brand = p.Brand,
                       LoaiId = p.LoaiId,
                       Category = p.Category,
                       GiaNhap = p.GiaNhap,
                       GiaBan = p.GiaBan,
                       GiaGiam = p.GiaGiam,
                       Image = p.Image,
                       ThongTinSanPham = p.ThongTinSanPham,
                       // Include Inventories navigation property
                       Inventory = p.Inventory.ToList()
                   });
            if (!string.IsNullOrEmpty(searchName))
            {
                products = products.Where(p => p.TenSanPham.Contains(searchName));
            }
            var pagedProducts = products.ToList();

            IPagedList<ProductModel> pagedProductsList = pagedProducts.ToPagedList(pageNumber, pageSize);
            ViewBag.SearchName = searchName;

            return View(pagedProductsList);
        }


        public IActionResult Logout()
        {
            // Đăng xuất người dùng bằng cách xóa phiên đăng nhập
            HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult ProductDetail(int productid)
        {
            var product = _context.Product
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Where(p => p.Id == productid)
                .FirstOrDefault();

            if (product == null)
            {
                // Xử lý trường hợp không tìm thấy sản phẩm
                return NotFound(); // Hoặc thực hiện xử lý khác theo yêu cầu của bạn.
            }

            var productView = new ProductViewDTO
            {
                Id = product.Id,
                GiaNhap = product.GiaNhap,
                HangTen = product.Brand.TenHang,
                Image = product.Image,
                MaSanPham = product.MaSanPham,
                TenSanPham = product.TenSanPham,
            };

            return View(productView);
        }
        public IActionResult ListCategory()
        {
            var categories = _context.Category.ToList();
            return Json(categories);
        }
        public IActionResult changeLanguage(String language)
        {
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(language)),
                new CookieOptions()
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1)
                });

            return Redirect(Request.Headers["Referer"].ToString());
        }

    }
}