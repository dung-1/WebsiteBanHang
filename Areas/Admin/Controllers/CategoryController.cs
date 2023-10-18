using Microsoft.AspNetCore.Mvc;
using WebsiteBanHang.Areas.Admin.Data;
using WebsiteBanHang.Areas.Admin.Models;
using X.PagedList;
using static WebsiteBanHang.Areas.Admin.Data.ApplicationDbContext;

namespace WebsiteBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
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
            int pageSize = 1; // Số mục trên mỗi trang

            var sortedCategories = _context.Category.OrderByDescending(c => c.Id).ToList();

            IPagedList<CategoryModel> pagedCategories = sortedCategories.ToPagedList(pageNumber, pageSize);

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
                _context.Category.Add(empobj);
                _context.SaveChanges();

                // Sắp xếp lại danh sách theo ID giảm dần (mới nhất lên đầu)
                var sortedCategories = _context.Category.OrderByDescending(c => c.Id).ToList();

                return View("Index", sortedCategories); // Trả về view "Index" với danh sách đã sắp xếp
            }

            return View(empobj);
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
                return RedirectToAction("Index");

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
            return RedirectToAction("Index");
        }


    }
}
