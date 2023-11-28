using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Areas.Admin.Data;
using WebsiteBanHang.Areas.Admin.Models;
using X.PagedList;

namespace WebsiteBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Employee")]

    public class CategoryController : Controller
    {               
        private readonly ApplicationDbContext _context;
        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? page, string searchName)
        {
            var pageNumber = page ?? 1; // Số trang mặc định (trang 1)
            int pageSize = 5; // Số mục trên mỗi trang

            var sortedBrands = _context.Category.AsQueryable().OrderByDescending(b => b.Id);

            if (!string.IsNullOrEmpty(searchName))
            {
                sortedBrands = (IOrderedQueryable<CategoryModel>)sortedBrands.Where(p => p.TenLoai.Contains(searchName));
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


            IPagedList<CategoryModel> pagedBrands = sortedProducts.ToPagedList(pageNumber, pageSize);


            if (TempData.ContainsKey("SuccessMessage"))
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }

            return View(pagedBrands);
        }





        public IActionResult Create()
        {
            return PartialView("_CategoryCreate");
        }


        [HttpGet]
        public JsonResult IsTenLoaiExists(string tenLoai)
        {
            bool isTenLoaiExists = _context.Category.Any(u => u.TenLoai == tenLoai);
            return Json(new { exists = isTenLoaiExists });
        }

        [HttpPost]
        public IActionResult Create(CategoryModel empobj)
        {
            ModelState.Remove("MaLoai");

            // Kiểm tra xem tên loại đã tồn tại chưa
            bool isTenLoaiExists = _context.Category.Any(u => u.TenLoai == empobj.TenLoai);

            if (isTenLoaiExists)
            {
                ModelState.AddModelError("TenLoai", "Tên loại sản phẩm đã tồn tại.");
                return View(empobj);
            }

            if (ModelState.IsValid)
            {
                // Tạo mã loại sản phẩm mới tự động và gán cho empobj.MaLoai
                empobj.MaLoai = GenerateCategoryCode(empobj);

                _context.Category.Add(empobj);
                _context.SaveChanges();

                return RedirectToAction("Index"); // Chuyển đến action "Index"
            }

            return View(empobj);
        }

        private string GenerateCategoryCode(CategoryModel empobj)
        {
            // Get the latest category code from the database
            var latestCategory = _context.Category
                .OrderByDescending(category => category.MaLoai) 
                .FirstOrDefault();

            if (latestCategory != null)
            {
                // Extract the numeric part of the latest category code
                if (int.TryParse(latestCategory.MaLoai.Substring(3), out int latestCategoryNumber))
                {
                    // Increment the category number and format it as VPxxxxx
                    return "LSP" + (latestCategoryNumber + 1).ToString("D5");
                }
            }

            // If no existing categories, start from VP00001
            return "LSP00001";
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = _context.Category.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            return PartialView("_CategoryEdit", category);
        }

        [HttpPost]
        public IActionResult Edit( [FromBody] CategoryModel empobj)
        {
            if (ModelState.IsValid)
            {
                _context.Category.Update(empobj);
                _context.SaveChanges();
                return RedirectToAction("Index"); // Sử dụng RedirectToAction để trả về action "Index"
            }

            return View(empobj);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var deleterecord = _context.Category.Find(id);
            if (deleterecord == null)
            {
                return NotFound();
            }

            try
            {
                _context.Category.Remove(deleterecord);
                _context.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Log lỗi hoặc xử lý nếu cần
                return Json(new { success = false });
            }
        }
     

    }
}
