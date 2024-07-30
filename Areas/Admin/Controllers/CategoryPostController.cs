using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using WebsiteBanHang.Areas.Admin.AdminDTO;
using WebsiteBanHang.Areas.Admin.Common;
using WebsiteBanHang.Areas.Admin.Data;
using WebsiteBanHang.Areas.Admin.Models;
using WebsiteBanHang.Models;
using X.PagedList;
using static System.Web.Razor.Parser.SyntaxConstants;

namespace WebsiteBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Employee")]

    public class CategoryPostController : Controller
    {
        private readonly ApplicationDbContext _context;
        readonly IReporting _IReporting;
        readonly AdminHomeController _homeAdmin;

        public CategoryPostController(ApplicationDbContext context, IReporting iReporting, AdminHomeController homeAdmin)
        {
            _context = context;
            _IReporting = iReporting;
            _homeAdmin = homeAdmin;

        }
        public IActionResult Index(int? page, string searchName)
        {
            try
            {
                var pageNumber = page ?? 1; // Số trang mặc định (trang 1)
                int pageSize = int.MaxValue; // Số mục trên mỗi trang

                var sortedBrands = _context.CategoryPost.AsQueryable().OrderByDescending(b => b.ModifiedTime);

                if (!string.IsNullOrEmpty(searchName))
                {
                    sortedBrands = (IOrderedQueryable<CategoryPostModel>)sortedBrands.Where(p => p.Name.Contains(searchName));
                }

                var sortedProducts = sortedBrands.ToList();

                if (searchName != null)
                {
                    ViewBag.SearchName = searchName;
                }
                else
                {
                    ViewBag.SearchName = ""; // Hoặc gán một giá trị mặc định khác nếu cần thiết
                }


                IPagedList<CategoryPostModel> pagedBrands = sortedProducts.ToPagedList(pageNumber, pageSize);


                if (TempData.ContainsKey("SuccessMessage"))
                {
                    ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
                }

                return View(pagedBrands);

            }
            catch (Exception ex)
            {
                return View("~/Areas/Admin/Views/Shared/_ErrorAdmin.cshtml");


            }

        }

        [HttpGet]
        public JsonResult IsTenLoaiExists(string tenLoai)
        {
            bool isTenLoaiExists = _context.CategoryPost.Any(u => u.Name  == tenLoai);
            return Json(new { exists = isTenLoaiExists });
        }

        private string GenerateCategoryCode(CategoryPostModel empobj)
        {
            // Get the latest category code from the database
            var latestCategory = _context.CategoryPost
                .OrderByDescending(category => category.MaTheLoai)
                .FirstOrDefault();

            if (latestCategory != null)
            {
                // Extract the numeric part of the latest category code
                if (int.TryParse(latestCategory.MaTheLoai.Substring(3), out int latestCategoryNumber))
                {
                    // Increment the category number and format it as VPxxxxx
                    return "LBV" + (latestCategoryNumber + 1).ToString("D5");
                }
            }

            // If no existing categories, start from VP00001
            return "LBV00001";
        }


        public IActionResult Create()
        {
            return PartialView("_CategoryPostCreate");
        }


        [HttpPost]
        public IActionResult Create(CategoryPostModel empobj)
        {

            try
            {
                ModelState.Remove("MaTheLoai");

                // Kiểm tra xem tên loại đã tồn tại chưa
                bool isTenLoaiExists = _context.CategoryPost.Any(u => u.Name == empobj.Name);

                if (isTenLoaiExists)
                {
                    ModelState.AddModelError("TenTheLoai", "Tên thể loại bài viết đã tồn tại.");
                    return View(empobj);
                }

                if (ModelState.IsValid)
                {
                    empobj.MaTheLoai = GenerateCategoryCode(empobj);
                    empobj.CreatedTime = DateTime.Now;
                    empobj.ModifiedTime = DateTime.Now;

                    _context.CategoryPost.Add(empobj);
                    _context.SaveChanges();

                    return RedirectToAction("Index"); 
                }

                return View(empobj);
            }
            catch (Exception ex)
            {
                return View("~/Areas/Admin/Views/Shared/_ErrorAdmin.cshtml");
            }


        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = _context.CategoryPost.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            return PartialView("_CategoryPostEdit", category);
        }

        [HttpPost]
        public IActionResult Edit([FromBody] CategoryPostModel empobj)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    empobj.ModifiedTime = DateTime.Now;
                    _context.CategoryPost.Update(empobj);
                    _context.SaveChanges();
                    return RedirectToAction("Index"); // Sử dụng RedirectToAction để trả về action "Index"
                }

                return View(empobj);
            }
            catch (Exception ex)
            {
                return View("~/Areas/Admin/Views/Shared/_ErrorAdmin.cshtml");


            }

        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var deleterecord = _context.CategoryPost.Find(id);
            if (deleterecord == null)
            {
                return View("~/Areas/Admin/Views/Shared/_ErrorAdmin.cshtml");
            }

            try
            {
                _context.CategoryPost.Remove(deleterecord);
                _context.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return View("~/Areas/Admin/Views/Shared/_ErrorAdmin.cshtml");

            }
        }

    }
}
