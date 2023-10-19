using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using WebsiteBanHang.Areas.Admin.Data;
using WebsiteBanHang.Areas.Admin.Models;
using WebsiteBanHang.Models;
using static WebsiteBanHang.Areas.Admin.Data.ApplicationDbContext;
using X.PagedList;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IActionResult> Index(int? page, string searchName, string selectedCategory)
        {
            var pageNumber = page ?? 1;
            int pageSize = 8;

            IQueryable<ProductModel> products = _context.Product;

            if (!string.IsNullOrEmpty(searchName))
            {
                products = products.Where(p => p.TenSanPham.Contains(searchName));
            }

            if (!string.IsNullOrEmpty(selectedCategory) && selectedCategory != "All")
            {
                products = products.Where(p => p.Category.TenLoai == selectedCategory);
            }

            IPagedList<ProductModel> pagedProducts = products.ToPagedList(pageNumber, pageSize);
            ViewBag.SearchName = searchName;

            // Lấy danh sách loại sản phẩm để truyền vào view
            var categories = _context.Category.Select(c => c.TenLoai).Distinct().ToList();
            ViewBag.Categories = categories;

            return View(pagedProducts);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
   

    }
}