﻿using Microsoft.AspNetCore.Mvc;
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
using static WebsiteBanHang.Areas.Admin.AdminDTO.ProductViewDTO;
using WebsiteBanHang.Areas.Admin.Common;

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
            int pageSize = 8;

            var products = _context.Product
                   .Where(p => p.Status == StatusActivity.Active)
                   .OrderByDescending(p => p.ModifiedTime)
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

        public async Task<IActionResult> ListProductSale(int? page, string searchName, string selectedCategory)
        {
            var pageNumber = page ?? 1;
            int pageSize = 8;

            var products = _context.Product
                .Where(p => p.GiaGiam > 0)
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

        public async Task<IActionResult> PostsDetail(int postsId)
        {
            try
            {

                var posts = await _context.Posts
           .Where(p => p.Id == postsId)
           .FirstOrDefaultAsync();

                if (posts == null)
                {
                    return NotFound();
                }

                posts.ViewCount += 1;

                _context.Posts.Update(posts);
                await _context.SaveChangesAsync();

                var post = await _context.Posts
                        .Where(p => p.Id == postsId)
                        .Select(p => new PostsViewDto
                        {
                            Id = p.Id,
                            ExcerptImage = p.ExcerptImage,
                            CreatedTime = p.CreatedTime,
                            Title = p.Title,
                            Content = p.Content,
                            CategoryName = p.Category.Name,
                            ViewCount = p.ViewCount
                        })
                        .FirstOrDefaultAsync();

                if (post == null)
                {
                    return NotFound();
                }

                var topHighlightedPosts = await _context.Posts
                    .Where(p => p.Status == StatusActivity.Active && p.FromDate <= DateTime.Now && p.ToDate >= DateTime.Now)
                        .OrderByDescending(p => p.ViewCount)
                        .Take(5)
                        .Select(p => new HighlightedPostDto
                        {
                            Id = p.Id,
                            Title = p.Title,
                            ExcerptImage = p.ExcerptImage
                        })
                        .ToListAsync();

                var topPromotionalPosts = await _context.Posts
                        .Where(p => p.Category.Name == "Khuyễn Mãi" && p.Status == StatusActivity.Active && p.FromDate <= DateTime.Now && p.ToDate >= DateTime.Now)
                        .OrderByDescending(p => p.CreatedTime)
                        .Take(10)
                        .Select(p => new PromotionalPostDto
                        {
                            Id = p.Id,
                            Title = p.Title,
                            ExcerptImage = p.ExcerptImage,
                            CreatedTime = p.CreatedTime
                        })
                        .ToListAsync();

                var latestPromotionalPost = topPromotionalPosts.FirstOrDefault();

                var viewModel = new PostsDetailViewModel
                {
                    Post = post,
                    TopHighlightedPosts = topHighlightedPosts,
                    TopPromotionalPosts = topPromotionalPosts.Skip(1).ToList(),
                    LatestPromotionalPost = latestPromotionalPost
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                return View();
            }
        }


    }
}