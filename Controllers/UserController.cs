using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebsiteBanHang.Areas.Admin.Data;
using WebsiteBanHang.Areas.Admin.Models;
using WebsiteBanHang.Models;
using X.PagedList;
using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Areas.Admin.AdminDTO;
using Microsoft.AspNetCore.Authorization;
using System.Dynamic;
using Microsoft.CSharp.RuntimeBinder;
using static WebsiteBanHang.Areas.Admin.AdminDTO.ProductViewDTO;
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
                .Include(p => p.Inventory)
                .Include(p => p.Comments)
                .ThenInclude(c => c.User)
                .Include(p => p.Order_Detai)
                .ThenInclude(od => od.order)
                .Where(p => p.Id == productid)
                .FirstOrDefault();

            if (product == null)
            {
                return NotFound();
            }

            var categoryId = product.Category.Id;
            var relatedProducts = _context.Product
                .Where(p => p.Category.Id == categoryId && p.Id != productid)
                .Select(p => new ProductDTO
                {
                    Id = p.Id,
                    MaSanPham = p.MaSanPham,
                    TenSanPham = p.TenSanPham,
                    GiaBan = p.GiaBan,
                    GiaGiam = p.GiaGiam,
                    GiaNhap = p.GiaNhap,
                    SoLuong = p.Inventory.FirstOrDefault() != null ? p.Inventory.FirstOrDefault().SoLuong : 0,
                    Image = p.Image
                })
                .ToList();

            var totalRating = product.Comments.Sum(c => c.Rating);
            var totalComments = product.Comments.Count;
            var averageRating = totalComments > 0 ? (double)totalRating / totalComments : 0;

            var totalSoldProducts = _context.Order_Detai
                .Where(od => od.ProductId == productid && od.order.trangThai == "Hoàn thành")
                .Sum(od => od.soLuong);

            var productView = new ProductViewDTO
            {
                Id = product.Id,
                GiaNhap = product.GiaNhap,
                GiaGiam = product.GiaGiam,
                GiaBan = product.GiaBan,
                HangTen = product.Brand.TenHang,
                LoaiTen = product.Category.TenLoai,
                Image = product.Image,
                MaSanPham = product.MaSanPham,
                TenSanPham = product.TenSanPham,
                ThongTinSanPham = product.ThongTinSanPham,
                RelatedProducts = relatedProducts,
                Comments = product.Comments.Select(c => new CommentDTO
                {
                    UserName = c.User.Email, // Giả sử MaNguoiDung là tên người dùng
                    Content = c.Content,
                    Rating = c.Rating,
                    CommentDate = c.CommentDate
                }).ToList(),
                RelatedInfoNumberProduct = new List<ProductViewDTO.InfoNumberProductDTO>
        {
            new ProductViewDTO.InfoNumberProductDTO
            {
                Rating = (int)Math.Round(averageRating),
                Comment = totalComments,
                ProductBuyNumber = totalSoldProducts
            }
        }
            };

            return View(productView);
        }

        [HttpPost]
        public IActionResult AddComment(int ProductId, int Rating, string Content)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account"); // Chuyển hướng đến trang đăng nhập nếu chưa đăng nhập
            }

            var userId = User.FindFirst("UserId")?.Value;
            if (userId == null)
            {
                return BadRequest("User not found");
            }

            var comment = new CommentModel
            {
                ProductId = ProductId,
                UserId = int.Parse(userId),
                Content = Content,
                Rating = Rating,
                CommentDate = DateTime.Now
            };

            _context.Comments.Add(comment);
            _context.SaveChanges();

            return RedirectToAction("ProductDetail", new { productid = ProductId });
        }
    }
}