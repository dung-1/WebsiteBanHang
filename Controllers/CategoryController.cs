using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;
using WebsiteBanHang.Areas.Admin.AdminDTO;
using WebsiteBanHang.Areas.Admin.Data;
using WebsiteBanHang.Areas.Admin.Models;
using X.PagedList;
using static WebsiteBanHang.Areas.Admin.AdminDTO.ProductViewDTO;

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
            // Find the category based on categoryid
            var category = _context.Category
                .Include(c => c.Prodcut)
                .FirstOrDefault(c => c.Id == categoryid);

            if (category == null)
            {
                return NotFound();
            }

            var categoryId = category.Id;

            // Get all products in the category by default (all checkboxes unchecked)
            var productsInCategory = _context.Product
                .Where(p => p.Category.Id == categoryId)
                .Select(p => new ProductViewDTO
                {
                    Id = p.Id,
                    MaSanPham = p.MaSanPham,
                    TenSanPham = p.TenSanPham,
                    HangTen = p.Category.TenLoai, // Brand name
                    LoaiTen = p.Category.TenLoai, // Category name
                    GiaNhap = p.GiaNhap,
                    GiaBan = p.GiaBan,
                    GiaGiam = p.GiaGiam,
                    Image = p.Image,
                    ThongTinSanPham = p.ThongTinSanPham,

                    // Checkboxes for filtering (set to unchecked by default)
                    CheckboxAll = false,
                    CheckboxPrice1 = false,
                    CheckboxPrice2 = false,
                    CheckboxPrice3 = false,
                    CheckboxPrice4 = false,
                    CheckboxPrice5 = false,

                    // Retrieve related products for each product
                    RelatedProducts = _context.Product
                        .Where(related => related.Id == p.Id)
                        .Select(related => new ProductDTO
                        {
                            Id = related.Id,
                            MaSanPham = related.MaSanPham,
                            TenSanPham = related.TenSanPham,
                            SoLuong = related.Inventory.FirstOrDefault() != null ? related.Inventory.FirstOrDefault().SoLuong : 0,
                            GiaNhap = related.GiaNhap,
                            GiaBan = related.GiaBan,
                            GiaGiam = related.GiaGiam,
                            Image = related.Image
                        })
                        .ToList()
                })
                .ToList();

            // If any checkbox is selected, update the corresponding Checkbox property in ProductViewDTO
            if (checkbox_all)
            {
                productsInCategory.ForEach(p => p.CheckboxAll = true);
            }
            else
            {
                productsInCategory.ForEach(p => p.CheckboxAll = false);
            }

            productsInCategory.ForEach(p =>
            {
                p.CheckboxPrice1 = checkbox_price_1;
                p.CheckboxPrice2 = checkbox_price_2;
                // ... update other Checkbox properties based on checkbox values
            });

            int pageSize = 9; // Number of products per page
            int pageNumber = page ?? 1; // Default page is 1

            var pagedProducts = productsInCategory.ToPagedList(pageNumber, pageSize);
            ViewBag.TotalProducts = productsInCategory.Count; // Pass total product count to ViewBag

            return View(pagedProducts);
        }


    }
}
