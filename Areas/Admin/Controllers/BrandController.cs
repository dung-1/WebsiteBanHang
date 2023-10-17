using Microsoft.AspNetCore.Mvc;
using WebsiteBanHang.Areas.Admin.Models;
using static WebsiteBanHang.Data.ApplicaitonDbContext;

namespace WebsiteBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BrandController : Controller
    {
        private readonly ApplicationDbContext _context;
        public BrandController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var categories = _context.HangSanPham.ToList();
            return View(categories);
        }
        public IActionResult Create()
        {
            return PartialView("_BrandCreate");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BrandModel empobj)
        {
            if (ModelState.IsValid)
            {
                _context.HangSanPham.Add(empobj);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(empobj);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = _context.HangSanPham.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            return PartialView("_BrandEdit", category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(BrandModel empobj)
        {
            if (ModelState.IsValid)
            {
                _context.HangSanPham.Update(empobj);
                _context.SaveChanges();
                return RedirectToAction("Index");

            }

            return View(empobj);
        }

        public IActionResult Delete(int? id)
        {
            var deleterecord = _context.HangSanPham.Find(id);
            if (deleterecord == null)
            {
                return NotFound();
            }
            _context.HangSanPham.Remove(deleterecord);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
