    using Microsoft.AspNetCore.Mvc;
using WebsiteBanHang.Areas.Admin.Models;
using static WebsiteBanHang.Data.ApplicaitonDbContext;

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
            var categories = _context.LoaiSanPham.ToList();
            return View(categories);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CategoryModel empobj)
        {
            if (ModelState.IsValid)
            {
                _context.LoaiSanPham.Add(empobj);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(empobj);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = _context.LoaiSanPham.Find(id);
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
                _context.LoaiSanPham.Update(empobj);
                _context.SaveChanges();
                return RedirectToAction("Index");

            }

            return View(empobj);
        }
       
        public IActionResult Delete(int? id)
        {
            var deleterecord = _context.LoaiSanPham.Find(id);
            if (deleterecord == null)
            {
                return NotFound();
            }
            _context.LoaiSanPham.Remove(deleterecord);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }


    }
}
