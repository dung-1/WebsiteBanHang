using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Areas.Admin.AdminDTO;
using WebsiteBanHang.Areas.Admin.Data;
using WebsiteBanHang.Areas.Admin.Models;
using static WebsiteBanHang.Areas.Admin.Data.ApplicationDbContext;

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
            var products = _context.Product
            .Include(p => p.Brand) // Eager load Brand
            .Include(p => p.Category) // Eager load Category
            .ToList();
            var data = products.Select(e => new ProductViewDTO
            {
                Id = e.Id,
                Gia = e.gia,
                HangTen = e.Brand.TenHang,
                LoaiTen = e.Category.TenLoai,
                Image = e.Image,
                MaSanPham = e.MaSanPham,
                TenSanPham = e.TenSanPham,
                ThongTinSanPham = e.ThongTinSanPham

            });
            return View(data);
        }

        public IActionResult Create()
        {
            // Truy vấn danh sách loại sản phẩm và hãng sản phẩm từ cơ sở dữ liệu
            var loaiProductList = _context.Category.ToList();
            var hangProductList = _context.Brand.ToList();

            // Tạo SelectList để sử dụng trong dropdown
            ViewBag.LoaiProductList = new SelectList(loaiProductList, "Id", "TenLoai");
            ViewBag.HangProductList = new SelectList(hangProductList, "Id", "TenHang");

            return PartialView("_ProductCreate");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductModel empobj)
        {
            if (ModelState.IsValid)
            {
                _context.Product.Add(empobj);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(empobj);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = _context.Product.Find(id);
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
                _context.Product.Update(empobj);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(empobj);
        }

        public IActionResult Delete(int? id)
        {
            var deleterecord = _context.Product.Find(id);
            if (deleterecord == null)
            {
                return NotFound();
            }
            _context.Product.Remove(deleterecord);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
