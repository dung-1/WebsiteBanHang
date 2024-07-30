using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Areas.Admin.AdminDTO;
using WebsiteBanHang.Areas.Admin.Common;
using WebsiteBanHang.Areas.Admin.Data;
using WebsiteBanHang.Areas.Admin.Models;
using X.PagedList;
using static WebsiteBanHang.Areas.Admin.Data.ApplicationDbContext;

namespace WebsiteBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class PostController : Controller
    {
        private readonly ILogger<PostController> _logger;

        private readonly ApplicationDbContext _context;
        readonly IReporting _IReporting;
        private readonly IWebHostEnvironment _hostEnvironment;
        readonly AdminHomeController _homeAdmin;

        public PostController(ApplicationDbContext context, ILogger<PostController> logger, IReporting iReporting, AdminHomeController homeAdmin, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _IReporting = iReporting;
            _homeAdmin = homeAdmin;
            _logger = logger;
            _hostEnvironment = hostEnvironment;
        }
        [Authorize(Roles = "Admin,Employee")]

        public IActionResult Index(int? page, string searchName)
        {
            try
            {
                var pageNumber = page ?? 1;
                int pageSize = int.MaxValue; 
                var postsQuery = _context.Posts
                    .Include(p => p.Category)
                    .OrderByDescending(p => p.ModifiedTime);

                if (!string.IsNullOrEmpty(searchName))
                {
                    postsQuery = (IOrderedQueryable<PostsModel>)postsQuery.Where(p => p.Title.Contains(searchName) || p.Category.Name.Contains(searchName));
                }

                var sortedPosts = postsQuery.ToList();

                if (searchName != null)
                {
                    ViewBag.SearchName = searchName;
                }
                else
                {
                    ViewBag.SearchName = ""; 
                }

                IPagedList<PostsViewDto> pagedPosts = sortedPosts
                    .Select(e => new PostsViewDto
                    {
                        Id = e.Id,
                        Title = e.Title,
                        CategoryName = e.Category.Name,
                        ViewCount = e.ViewCount,
                        Status = e.Status,
                        FromDate = e.FromDate,
                        ToDate = e.ToDate,
                    })
                    .ToPagedList(pageNumber, pageSize);

                return View(pagedPosts);
            }
            catch (Exception)
            {
                return View("~/Areas/Admin/Views/Shared/_ErrorAdmin.cshtml");
            }

        }

        [HttpGet]
        public JsonResult IsTitleExists(string title)
        {
            bool isPostNamexists = _context.Posts.Any(p => p.Title == title);
            return Json(new { exists = isPostNamexists });
        }

        [HttpGet]
        public JsonResult IsTitleExist(string title, int currentPostId)
        {
            bool isTenSanPhamExists = _context.Posts.Any(p => p.Title == title && p.Id != currentPostId);
            return Json(new { exists = isTenSanPhamExists });
        }


        private string GeneratePostCode(PostsModel posts)
        {
            var latestPosts= _context.Posts
                .OrderByDescending(p => p.MaBaiViet)
                .FirstOrDefault();

            if (latestPosts != null)
            {
                if (int.TryParse(latestPosts.MaBaiViet.Substring(3), out int latestPostsNumber))
                {
                    return "MBV" + (latestPostsNumber + 1).ToString("D5");
                }
            }

            return "MBV00001";
        }

        public IActionResult _PostsAdd()
        {
            try
            {
                var Categories = _context.CategoryPost.ToList();
                ViewBag.Categories = new SelectList(Categories, "Id", "Name");
                return View();
            }
            catch
            {
                return View("~/Areas/Admin/Views/Shared/_ErrorAdmin.cshtml");
            }
        }


        [HttpPost]
        public IActionResult Create(PostsModel posts, IFormFile imageFile)
        {
            try
            {
                bool isPostNamexists = _context.Posts.Any(p => p.Title == posts.Title);
                if (isPostNamexists)
                {
                    ModelState.AddModelError("title", "Tiêu đề bài viết đã tồn tại.");
                    return View(posts);
                }
                posts.MaBaiViet = GeneratePostCode(posts);

                var category = _context.CategoryPost.Find(posts.CategoryId);

                if ( category != null)
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var imagePath = "images/";
                        var imageName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagePath, imageName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            imageFile.CopyTo(stream);
                        }
                        posts.ExcerptImage = Path.Combine(imagePath, imageName);
                        posts.Category = category;
                        posts.CreatedTime = DateTime.Now;
                        posts.ModifiedTime = DateTime.Now;

                        _context.Posts.Add(posts);
                        _context.SaveChanges();

                        return RedirectToAction("Index");
                    }
                }
                return View(posts);
            }
            catch (Exception)
            {
                return View("~/Areas/Admin/Views/Shared/_ErrorAdmin.cshtml");
            }

        }




        [HttpGet]
        public IActionResult _PostsUpdate(int id)
        {
            var category = _context.Posts.Find(id);
            var Categories = _context.CategoryPost.ToList();
            ViewBag.Categories = new SelectList(Categories, "Id", "Name");
            if (category == null)
            {
                return View("~/Areas/Admin/Views/Shared/_ErrorAdmin.cshtml");
            }
            return View(category);
        }


        [HttpPost]
        public IActionResult Edit(PostsModel updatedPosts, IFormFile imageFile)
        {

            try
            {
                bool isTenSanPhamExists = _context.Posts.Any(p => p.Title == updatedPosts.Title && p.Id != updatedPosts.Id);

                if (isTenSanPhamExists)
                {
                    ModelState.AddModelError("TenSanPham", "Tên sản phẩm đã tồn tại.");
                    return View(updatedPosts);
                }

                var category = _context.CategoryPost.Find(updatedPosts.CategoryId);
                var existingPosts = _context.Posts.Find(updatedPosts.Id);

                if (existingPosts != null)
                {
                    if (imageFile != null && imageFile.Length > 0)

                    {
                        // Nếu có tệp ảnh mới được tải lên, cập nhật ảnh
                        var imagePath = "images/";
                        var imageName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagePath, imageName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            imageFile.CopyTo(stream);
                        }

                        existingPosts.ExcerptImage = Path.Combine(imagePath, imageName);
                    }
                    // Cập nhật các thông tin khác của sản phẩm
                    existingPosts.Id = updatedPosts.Id;
                    existingPosts.MaBaiViet = updatedPosts.MaBaiViet;
                    existingPosts.Title = updatedPosts.Title;
                    existingPosts.Content = updatedPosts.Content;
                    existingPosts.CategoryId = updatedPosts.CategoryId;
                    existingPosts.FromDate = updatedPosts.FromDate;
                    existingPosts.ToDate = updatedPosts.ToDate;
                    existingPosts.Status = updatedPosts.Status;
                    updatedPosts.Category = category;
                    existingPosts.ModifiedTime = DateTime.Now;
                    _context.Posts.Update(existingPosts);
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }

                // Nếu ModelState không hợp lệ hoặc không tìm thấy sản phẩm, quay lại view Edit với model đã nhập
                return View(updatedPosts);
            }
            catch (Exception ex)
            {
                return View("~/Areas/Admin/Views/Shared/_ErrorAdmin.cshtml");
            }

        }

        public IActionResult Delete(int? id)
        {
            var deleterecord = _context.Posts.Find(id);
            if (deleterecord == null)
            {
                return View("~/Areas/Admin/Views/Shared/_ErrorAdmin.cshtml");
            }

            try
            {
                _context.Posts.Remove(deleterecord);
                _context.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Log lỗi hoặc xử lý nếu cần
                return View("~/Areas/Admin/Views/Shared/_ErrorAdmin.cshtml");
            }
        }


    }
}