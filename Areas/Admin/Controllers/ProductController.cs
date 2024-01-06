using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using WebsiteBanHang.Areas.Admin.AdminDTO;
using WebsiteBanHang.Areas.Admin.Data;
using WebsiteBanHang.Areas.Admin.Models;
using X.PagedList;
using static WebsiteBanHang.Areas.Admin.Data.ApplicationDbContext;

namespace WebsiteBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;

        private readonly ApplicationDbContext _context;
        public ProductController(ApplicationDbContext context, ILogger<ProductController> logger)
        {
            _context = context;
            _logger = logger;

        }
        [Authorize(Roles = "Admin,Employee")]

        public IActionResult Index(int? page, string searchName)
        {
            var pageNumber = page ?? 1;
            int pageSize = 5;

            var productsQuery =  _context.Product
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .OrderByDescending(p => p.Id);

            if (!string.IsNullOrEmpty(searchName))
            {
                productsQuery = (IOrderedQueryable<ProductModel>)productsQuery.Where(p => p.TenSanPham.Contains(searchName) || p.Brand.TenHang.Contains(searchName) || p.Category.TenLoai.Contains(searchName));
            }

            var sortedProducts = productsQuery.ToList();

            if (searchName != null)
            {
                ViewBag.SearchName = searchName;
            }
            else
            {
                ViewBag.SearchName = ""; // Hoặc gán một giá trị mặc định khác nếu cần thiết
            }

            IPagedList<ProductViewDTO> pagedProducts = sortedProducts
                .Select(e => new ProductViewDTO
                {
                    Id = e.Id,
                    Gia = e.Gia,
                    HangTen = e.Brand.TenHang,
                    Image = e.Image,
                    MaSanPham = e.MaSanPham,
                    TenSanPham = e.TenSanPham,
                })
                .ToPagedList(pageNumber, pageSize);

            return View(pagedProducts);
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

        [HttpGet]
        public JsonResult IsTenSanPhamExists(string tenSanPham)
        {
            bool isTenSanPhamExists = _context.Product.Any(p => p.TenSanPham == tenSanPham);
            return Json(new { exists = isTenSanPhamExists });
        }

        [HttpPost]
        public IActionResult Create(ProductModel product, IFormFile imageFile)
        {
            bool isTenSanPhamExists = _context.Product.Any(p => p.TenSanPham == product.TenSanPham);
            if (isTenSanPhamExists)
            {
                ModelState.AddModelError("TenSanPham", "Tên sản phẩm đã tồn tại.");
                return View(product);
            }

            // Tạo mã sản phẩm mới tự động và gán cho product.MaSanPham
            product.MaSanPham = GenerateProductCode(product);

            // Tìm hãng sản phẩm và loại sản phẩm dựa trên ID được chọn trong dropdownlist
            var brand = _context.Brand.Find(product.HangId);
            var category = _context.Category.Find(product.LoaiId);

            if (brand != null && category != null)
            {
                // Xử lý tải ảnh lên và lưu đường dẫn vào trường Image
                if (imageFile != null && imageFile.Length > 0)
                {
                    var imagePath = "images/";
                    var imageName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagePath, imageName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        imageFile.CopyTo(stream);
                    }

                    product.Image = Path.Combine(imagePath, imageName);

                    // Gán hãng sản phẩm và loại sản phẩm cho sản phẩm
                    product.Brand = brand;
                    product.Category = category;

                    // Thêm sản phẩm vào cơ sở dữ liệu
                    _context.Product.Add(product);
                    _context.SaveChanges();

                    return RedirectToAction("Index");
                }
            }

            // Nếu ModelState không hợp lệ hoặc không tìm thấy hãng sản phẩm hoặc loại sản phẩm, quay lại view Create với model đã nhập
            return View(product);
        }

        private string GenerateProductCode(ProductModel product)
        {
            // Tạo mã sản phẩm mới tự động và gán cho product.MaSanPham
            var latestProduct = _context.Product
                .OrderByDescending(p => p.MaSanPham)
                .FirstOrDefault();

            if (latestProduct != null)
            {
                // Extract the numeric part of the latest product code
                if (int.TryParse(latestProduct.MaSanPham.Substring(3), out int latestProductNumber))
                {
                    // Increment the product number and format it as MSPxxxxx
                    return "MSP" + (latestProductNumber + 1).ToString("D5");
                }
            }

            // If no existing products, start from MSP00001
            return "MSP00001";
        }



        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = _context.Product.Find(id);
            var loaiProductList = _context.Category.ToList();
            var hangProductList = _context.Brand.ToList();

            // Tạo SelectList để sử dụng trong dropdown
            ViewBag.LoaiProductList = new SelectList(loaiProductList, "Id", "TenLoai");
            ViewBag.HangProductList = new SelectList(hangProductList, "Id", "TenHang");
            if (category == null)
            {
                return NotFound();
            }
            return PartialView("_ProductEdit", category);
        }




        [HttpGet]
        public JsonResult IsTenSanPhamExist(string tenSanPham, int currentProductId)
        {
            bool isTenSanPhamExists = _context.Product.Any(p => p.TenSanPham == tenSanPham && p.Id != currentProductId);
            return Json(new { exists = isTenSanPhamExists });
        }


        [HttpPost]
        public IActionResult Edit(ProductModel updatedProduct, IFormFile imageFile)
        {
            bool isTenSanPhamExists = _context.Product.Any(p => p.TenSanPham == updatedProduct.TenSanPham && p.Id != updatedProduct.Id);

            if (isTenSanPhamExists)
            {
                ModelState.AddModelError("TenSanPham", "Tên sản phẩm đã tồn tại.");
                return View(updatedProduct);
            }

            var brand = _context.Brand.Find(updatedProduct.HangId);
            var category = _context.Category.Find(updatedProduct.LoaiId);
            var existingProduct = _context.Product.Find(updatedProduct.Id);

            if (existingProduct != null)
            {
                if (imageFile != null && imageFile.Length > 0)

                {
                    // Nếu có tệp ảnh mới được tải lên, cập nhật ảnh
                    var imagePath = "images/";
                    var imageName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagePath, imageName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        imageFile.CopyTo(stream);
                    }

                    existingProduct.Image = Path.Combine(imagePath, imageName);
                }
                // Cập nhật các thông tin khác của sản phẩm
                existingProduct.Id = updatedProduct.Id;
                existingProduct.MaSanPham = updatedProduct.MaSanPham;
                existingProduct.TenSanPham = updatedProduct.TenSanPham;
                existingProduct.ThongTinSanPham = updatedProduct.ThongTinSanPham;
                existingProduct.Gia = updatedProduct.Gia;
                existingProduct.GiaGiam = updatedProduct.GiaGiam;
                existingProduct.HangId = updatedProduct.HangId;
                existingProduct.LoaiId = updatedProduct.LoaiId;
                updatedProduct.Brand = brand;
                updatedProduct.Category = category;

                _context.Product.Update(existingProduct);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            // Nếu ModelState không hợp lệ hoặc không tìm thấy sản phẩm, quay lại view Edit với model đã nhập
            return View(updatedProduct);
        }

        public IActionResult Delete(int? id)
        {
            var deleterecord = _context.Product.Find(id);
            if (deleterecord == null)
            {
                return NotFound();
            }

            try
            {
                _context.Product.Remove(deleterecord);
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
