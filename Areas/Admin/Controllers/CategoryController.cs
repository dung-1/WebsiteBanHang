using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteBanHang.Areas.Admin.Data;
using WebsiteBanHang.Areas.Admin.Models;
using X.PagedList;
using static WebsiteBanHang.Areas.Admin.Data.ApplicationDbContext;

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

        public IActionResult Index(int? page)
        {
            var pageNumber = page ?? 1; // Số trang mặc định (trang 1)
            int pageSize = 5; // Số mục trên mỗi trang

            var sortedCategories = _context.Category.OrderByDescending(c => c.Id).ToList();

            IPagedList<CategoryModel> pagedCategories = sortedCategories.ToPagedList(pageNumber, pageSize);
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }
            return View(pagedCategories);
        }

        public IActionResult Create()
        {
            return PartialView("_CategoryCreate");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CategoryModel empobj)
        {
            if (ModelState.IsValid)
            {
                if (!IsMaLoaiUnique(empobj.MaLoai))
                {
                    ModelState.AddModelError("MaLoai", "Mã Loại Sản Phẩm đã tồn tại.");
                    return RedirectToAction("Index");
                }

                _context.Category.Add(empobj);
                _context.SaveChanges();

                return RedirectToAction("Index"); // Chuyển đến action "Index"
            }

            return View(empobj);
        }


        private bool IsMaLoaiUnique(string maLoai)
        {
            return !_context.Category.Any(c => c.MaLoai == maLoai);
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
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CategoryModel empobj)
        {
            if (ModelState.IsValid)
            {
                _context.Category.Update(empobj);
                _context.SaveChanges();
                return RedirectToAction("Index"); // Sử dụng RedirectToAction để trả về action "Index"
            }

            return View(empobj);
        }

        public IActionResult Delete(int? id)
        {
            var deleterecord = _context.Category.Find(id);
            if (deleterecord == null)
            {
                return NotFound();
            }
            _context.Category.Remove(deleterecord);
            _context.SaveChanges();
            return RedirectToAction("Index"); // Sử dụng RedirectToAction để trả về action "Index"
        }
    }
}
