    using Microsoft.AspNetCore.Mvc;
using WebsiteBanHang.Areas.Admin.Data;
using WebsiteBanHang.Areas.Admin.Models;
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
        public IActionResult Index()
        {
            var categories = _context.Category.ToList();
            return View(categories);
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
                return RedirectToAction("Index");
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
