using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebsiteBanHang.Areas.Admin.Data;
using WebsiteBanHang.Areas.Admin.Models;
using WebsiteBanHang.Models;
using X.PagedList;
using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Areas.Admin.AdminDTO;
using Microsoft.AspNetCore.Authorization;
namespace WebsiteBanHang.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        [Authorize(Roles = "Customer")]
        public IActionResult Index(int? page, string searchName, string selectedCategory)
        {
            var pageNumber = page ?? 1;
            int pageSize = 8;

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
                LoaiTen = product.Category.TenLoai,
                Image = product.Image,
                MaSanPham = product.MaSanPham,
                TenSanPham = product.TenSanPham,
                ThongTinSanPham = product.ThongTinSanPham
            };

            return View(productView);
        }


    }
}