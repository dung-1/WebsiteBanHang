using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;
using WebsiteBanHang.Areas.Admin.AdminDTO;
using WebsiteBanHang.Areas.Admin.Data;
using WebsiteBanHang.Areas.Admin.Models;
using X.PagedList;

namespace WebsiteBanHang.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CategoryController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult Index(int categoryid, int? page, bool checkbox_all, bool checkbox_price_1, bool checkbox_price_2, bool checkbox_price_3, bool checkbox_price_4, bool checkbox_price_5)
        {
            // Tìm danh mục dựa trên categoryid
            var category = _context.Category
                .Include(c => c.Prodcut)
                .FirstOrDefault(c => c.Id == categoryid);

            if (category == null)
            {
                return NotFound();
            }

            // Lấy danh sách sản phẩm của danh mục đó
            var productsInCategory = category.Prodcut
                .Where(p =>
                    (checkbox_all ||  // If checkbox_all is checked, include all products
                    (checkbox_price_1 && p.GiaGiam < 10000000) ||
                    (checkbox_price_2 && p.GiaGiam >= 10000000 && p.GiaGiam < 15000000) ||
                    (checkbox_price_3 && p.GiaGiam >= 15000000 && p.GiaGiam < 20000000) ||
                    (checkbox_price_4 && p.GiaGiam >= 20000000 && p.GiaGiam < 25000000) ||
                    (checkbox_price_5 && p.GiaGiam >= 25000000)))
                .Select(p => new ProductViewDTO
                {
                    Id = p.Id,
                    Gia = p.Gia,
                    LoaiTen = p.Category.TenLoai,
                    Image = p.Image,
                    MaSanPham = p.MaSanPham,
                    GiaGiam = p.GiaGiam,
                    TenSanPham = p.TenSanPham,
                    CheckboxAll = checkbox_all,
                    CheckboxPrice1 = checkbox_price_1,
                    // Add other Checkbox properties as needed
                })
                .ToList();

            int pageSize = 12; // Số sản phẩm trên mỗi trang
            int pageNumber = page ?? 1; // Trang mặc định là 1

            var pagedProducts = productsInCategory.ToPagedList(pageNumber, pageSize);
            ViewBag.TotalProducts = productsInCategory.Count; // Truyền số lượng sản phẩm vào ViewBag

            return View(pagedProducts);
        }







    }
}
