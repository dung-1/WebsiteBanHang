using Microsoft.AspNetCore.Mvc;
using WebsiteBanHang.Areas.Admin.Data;
using WebsiteBanHang.Areas.Admin.Models;
using static WebsiteBanHang.Areas.Admin.Data.ApplicationDbContext;

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
            var categories = _context.Brand.ToList();
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
                _context.Brand.Add(empobj);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(empobj);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = _context.Brand.Find(id);
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
                _context.Brand.Update(empobj);
                _context.SaveChanges();
                return RedirectToAction("Index");

            }

            return View(empobj);
        }

        public IActionResult Delete(int? id)
        {
            var deleterecord = _context.Brand.Find(id);
            if (deleterecord == null)
            {
                return NotFound();
            }
            _context.Brand.Remove(deleterecord);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
