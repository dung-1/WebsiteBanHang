using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebsiteBanHang.Areas.Admin.Models;
using static WebsiteBanHang.Data.ApplicaitonDbContext;

namespace WebsiteBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var categories = _context.SanPham.ToList();
            return View(categories);
        }
        public IActionResult Create()
        {
            // Truy vấn danh sách loại sản phẩm và hãng sản phẩm từ cơ sở dữ liệu
            var loaiSanPhamList = _context.LoaiSanPham.ToList();
            var hangSanPhamList = _context.HangSanPham.ToList();

            // Tạo SelectList để sử dụng trong dropdown
            ViewBag.LoaiSanPhamList = new SelectList(loaiSanPhamList, "Id", "tenLoai");
            ViewBag.HangSanPhamList = new SelectList(hangSanPhamList, "Id", "tenHang");

            return PartialView("_ProductCreate");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductModel empobj)
        {
            if (ModelState.IsValid)
            {
                _context.SanPham.Add(empobj);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(empobj);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = _context.SanPham.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            return PartialView("_ProductEdit", category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProductModel empobj)
        {
            if (ModelState.IsValid)
            {
                _context.SanPham.Update(empobj);
                _context.SaveChanges();
                return RedirectToAction("Index");

            }

            return View(empobj);
        }

        public IActionResult Delete(int? id)
        {
            var deleterecord = _context.SanPham.Find(id);
            if (deleterecord == null)
            {
                return NotFound();
            }
            _context.SanPham.Remove(deleterecord);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
