using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Globalization;
using WebsiteBanHang.Areas.Admin.AdminDTO;
using WebsiteBanHang.Areas.Admin.Common;
using WebsiteBanHang.Areas.Admin.Data;
using WebsiteBanHang.Areas.Admin.Models;
using WebsiteBanHang.Models;
using X.PagedList;
using static WebsiteBanHang.Areas.Admin.Data.ApplicationDbContext;

namespace WebsiteBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;

        private readonly ApplicationDbContext _context;
        readonly IReporting _IReporting;
        private readonly IWebHostEnvironment _hostEnvironment;
        readonly AdminHomeController _homeAdmin;
        public ProductController(ApplicationDbContext context, ILogger<ProductController> logger, IReporting iReporting, AdminHomeController homeAdmin, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _IReporting = iReporting;
            _homeAdmin = homeAdmin;
            _logger = logger;
            _hostEnvironment = hostEnvironment;
        }
        [Authorize(Roles = "Admin,Employee")]

        public IActionResult Index(int? page, string searchName)
        {
            try
            {
                var pageNumber = page ?? 1; // Số trang mặc định (trang 1)
                int pageSize = int.MaxValue; // Số mục trên mỗi trang
                var productsQuery = _context.Product
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .OrderByDescending(p => p.ModifiedTime);

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
                        GiaNhap = e.GiaNhap,
                        GiaBan = e.GiaBan,
                        GiaGiam = e.GiaGiam,
                        Status = e.Status,
                        HangTen = e.Brand.TenHang,
                        Image = e.Image,
                        MaSanPham = e.MaSanPham,
                        TenSanPham = e.TenSanPham,
                    })
                    .ToPagedList(pageNumber, pageSize);

                return View(pagedProducts);
            }
            catch (Exception)
            {
                return View("~/Areas/Admin/Views/Shared/_ErrorAdmin.cshtml");
            }

        }

        public IActionResult _ProductAdd()
        {
            // Truy vấn danh sách loại sản phẩm và hãng sản phẩm từ cơ sở dữ liệu
            try
            {
                var loaiProductList = _context.Category.ToList();
                var hangProductList = _context.Brand.ToList();

                // Tạo SelectList để sử dụng trong dropdown
                ViewBag.LoaiProductList = new SelectList(loaiProductList, "Id", "TenLoai");
                ViewBag.HangProductList = new SelectList(hangProductList, "Id", "TenHang");

                return View();
            }
            catch
            {

                return View("~/Areas/Admin/Views/Shared/_ErrorAdmin.cshtml");
            }
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
            try
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
                        product.CreatedTime = DateTime.Now;
                        product.ModifiedTime = DateTime.Now;

                        // Thêm sản phẩm vào cơ sở dữ liệu
                        _context.Product.Add(product);
                        _context.SaveChanges();

                        return RedirectToAction("Index");
                    }
                }

                // Nếu ModelState không hợp lệ hoặc không tìm thấy hãng sản phẩm hoặc loại sản phẩm, quay lại view Create với model đã nhập
                return View(product);
            }
            catch (Exception)
            {
                return View("~/Areas/Admin/Views/Shared/_ErrorAdmin.cshtml");
            }

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
        public IActionResult _ProductUpdate(int id)
        {
            var category = _context.Product.Find(id);
            var loaiProductList = _context.Category.ToList();
            var hangProductList = _context.Brand.ToList();

            // Tạo SelectList để sử dụng trong dropdown
            ViewBag.LoaiProductList = new SelectList(loaiProductList, "Id", "TenLoai");
            ViewBag.HangProductList = new SelectList(hangProductList, "Id", "TenHang");
            if (category == null)
            {
                return View("~/Areas/Admin/Views/Shared/_ErrorAdmin.cshtml");
            }
            return View(category);
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

            try
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
                    existingProduct.GiaBan = updatedProduct.GiaBan;
                    existingProduct.GiaNhap = updatedProduct.GiaNhap;
                    existingProduct.Status = updatedProduct.Status;
                    existingProduct.GiaGiam = updatedProduct.GiaGiam;
                    existingProduct.HangId = updatedProduct.HangId;
                    existingProduct.LoaiId = updatedProduct.LoaiId;
                    updatedProduct.Brand = brand;
                    updatedProduct.Category = category;
                    existingProduct.ModifiedTime = DateTime.Now;
                    _context.Product.Update(existingProduct);
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }

                // Nếu ModelState không hợp lệ hoặc không tìm thấy sản phẩm, quay lại view Edit với model đã nhập
                return View(updatedProduct);
            }
            catch (Exception ex)
            {
                return View("~/Areas/Admin/Views/Shared/_ErrorAdmin.cshtml");
            }

        }

        public IActionResult Delete(int? id)
        {
            var deleterecord = _context.Product.Find(id);
            if (deleterecord == null)
            {
                return View("~/Areas/Admin/Views/Shared/_ErrorAdmin.cshtml");
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
                return View("~/Areas/Admin/Views/Shared/_ErrorAdmin.cshtml");
            }
        }

        //import excel

        [HttpPost]
        public async Task<IActionResult> Import(IFormFile formFile, CancellationToken cancellationToken)
        {
            try
            {
                if (formFile == null || formFile.Length <= 0)
                {
                    return View("Error", new ErrorViewModel { RequestId = "formfile is empty" });
                }

                if (!Path.GetExtension(formFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    return View("Error", new ErrorViewModel { RequestId = "Not Support file extension" });
                }

                var productsToImport = new List<ProductModel>();

                using (var stream = new MemoryStream())
                {
                    await formFile.CopyToAsync(stream, cancellationToken);

                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        var rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            var maSanPham = worksheet.Cells[row, 1].Value?.ToString()?.Trim();
                            var tenSanPham = worksheet.Cells[row, 2].Value?.ToString()?.Trim();
                            var hangSanPham = int.TryParse(worksheet.Cells[row, 3].Value?.ToString()?.Trim(), out int hangId) ? hangId : 0;
                            var loaiSanPham = int.TryParse(worksheet.Cells[row, 4].Value?.ToString()?.Trim(), out int loaiId) ? loaiId : 0;
                            var giaNhap = int.TryParse(worksheet.Cells[row, 5].Value?.ToString()?.Trim(), out int giaNhapValue) ? giaNhapValue : 0;
                            var giaBan = int.TryParse(worksheet.Cells[row, 6].Value?.ToString()?.Trim(), out int giaBanValue) ? giaBanValue : 0;
                            var giamGia = int.TryParse(worksheet.Cells[row, 7].Value?.ToString()?.Trim(), out int giamGiaValue) ? giamGiaValue : 0;
                            var image = worksheet.Cells[row, 8].Value?.ToString()?.Trim();
                            var thongTin = worksheet.Cells[row, 9].Value?.ToString()?.Trim();

                            // Validate required fields
                            if (!string.IsNullOrEmpty(maSanPham) && !string.IsNullOrEmpty(tenSanPham))
                            {
                                // Check if product with the same name and code already exists
                                bool isProductExists = await _context.Product.AnyAsync(p => p.TenSanPham == tenSanPham && p.MaSanPham == maSanPham, cancellationToken);

                                if (!isProductExists)
                                {
                                    // Generate unique filename for image
                                    var imageFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(image);

                                    // Path to save image relative to wwwroot
                                    var imagePath = Path.Combine("images", imageFileName);

                                    // Combine with wwwroot path
                                    var absoluteImagePath = Path.Combine(_hostEnvironment.WebRootPath, imagePath);

                                    // Copy image to wwwroot/images folder
                                    using (var fileStream = new FileStream(absoluteImagePath, FileMode.Create))
                                    {
                                        await formFile.CopyToAsync(fileStream, cancellationToken);
                                    }

                                    var imageUrl = "" + imagePath.Replace('\\', '/'); // Use '/' for URL

                                    // Add product to list for import
                                    productsToImport.Add(new ProductModel
                                    {
                                        MaSanPham = maSanPham,
                                        TenSanPham = tenSanPham,
                                        HangId = hangSanPham,
                                        LoaiId = loaiSanPham,
                                        GiaNhap = giaNhap,
                                        GiaBan = giaBan,
                                        GiaGiam = giamGia,
                                        Image = imageUrl,
                                        ThongTinSanPham = thongTin,
                                    });
                                }
                            }
                        }
                    }
                }

                // Add imported products to the database
                if (productsToImport.Count > 0)
                {
                    _context.Product.AddRange(productsToImport);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "Error occurred during import");

                // Return an error view
                return View("~/Areas/Admin/Views/Shared/_ErrorAdmin.cshtml");
            }
        }



        [HttpPost]
        public IActionResult DownloadReport(IFormCollection obj)
        {
            string reportname = $"Dungcts_Product_{Guid.NewGuid():N}.xlsx";
            var list = _IReporting.GetProductwiseReport();
            if (list.Count > 0)
            {
                var exportbytes = _homeAdmin.ExporttoExcel<Product_exrepoting_Dto>(list, reportname);
                return File(exportbytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", reportname);
            }
            else
            {
                TempData["Message"] = "No Data to Export";
                return View();
            }
        }
    }
}
