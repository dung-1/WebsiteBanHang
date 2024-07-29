    using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Globalization;
using WebsiteBanHang.Areas.Admin.AdminDTO;
using WebsiteBanHang.Areas.Admin.Common;
using WebsiteBanHang.Areas.Admin.Data;
using WebsiteBanHang.Areas.Admin.Models;
using WebsiteBanHang.Models;
using X.PagedList;
using static i18n.Helpers.NuggetParser;

namespace WebsiteBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Employee")]

    public class BrandController : Controller
    {
        private readonly ApplicationDbContext _context;
        readonly IReporting _IReporting;
        readonly AdminHomeController _homeAdmin;
        public BrandController(ApplicationDbContext context, IReporting iReporting, AdminHomeController homeAdmin)
        {
            _context = context;
            _IReporting = iReporting;
            _homeAdmin = homeAdmin;

        }

        public IActionResult Index(int? page, string searchName)
        {
            try
            {
                var pageNumber = page ?? 1; // Số trang mặc định (trang 1)
                int pageSize = int.MaxValue; // Số mục trên mỗi trang

                var sortedBrands = _context.Brand.AsQueryable().OrderByDescending(b => b.ModifiedTime);

                if (!string.IsNullOrEmpty(searchName))
                {
                    sortedBrands = (IOrderedQueryable<BrandModel>)sortedBrands.Where(p => p.TenHang.Contains(searchName));
                }

                var sortedProducts = sortedBrands.ToList();

                if (searchName != null)
                {
                    ViewBag.SearchName = searchName;
                }
                else
                {
                    ViewBag.SearchName = ""; // Hoặc gán một giá trị mặc định khác nếu cần thiết
                }


                IPagedList<BrandModel> pagedBrands = sortedProducts.ToPagedList(pageNumber, pageSize);



                if (TempData.ContainsKey("SuccessMessage"))
                {
                    ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
                }

                return View(pagedBrands);

            }
            catch
            {
                return View("~/Areas/Admin/Views/Shared/_ErrorAdmin.cshtml");

            }

        }


        public IActionResult Create()
        {
            return PartialView("_BrandCreate");
        }

        [HttpGet]
        public JsonResult IsTenHangExists(string tenHang)
        {
            bool isTenHangExists = _context.Brand.Any(b => b.TenHang == tenHang);
            return Json(new { exists = isTenHangExists });
        }

        [HttpPost]
        public IActionResult Create(BrandModel empobj)
        {
            ModelState.Remove("MaHang");

            // Kiểm tra xem tên hãng đã tồn tại chưa
            bool isTenHangExists = _context.Brand.Any(b => b.TenHang == empobj.TenHang);

            if (isTenHangExists)
            {
                ModelState.AddModelError("TenHang", "Tên hãng đã tồn tại.");
                return RedirectToAction("Index"); // Chuyển đến action "Index"
            }

            if (ModelState.IsValid)
            {
                // Tạo mã hãng mới tự động và gán cho empobj.MaHang
                empobj.MaHang = GenerateBrandCode(empobj);
                empobj.CreatedTime = DateTime.Now;
                empobj.ModifiedTime = DateTime.Now;

                _context.Brand.Add(empobj);
                _context.SaveChanges();

                return RedirectToAction("Index"); // Chuyển đến action "Index"
            }

            // Nếu có lỗi, trả về view với model và hiển thị thông báo lỗi
            return RedirectToAction("Index");
        }

        private string GenerateBrandCode(BrandModel empobj)
        {
            // Get the latest brand code from the database
            var latestBrand = _context.Brand
                .OrderByDescending(brand => brand.MaHang)
                .FirstOrDefault();

            if (latestBrand != null)
            {
                // Extract the numeric part of the latest brand code
                if (int.TryParse(latestBrand.MaHang.Substring(3), out int latestBrandNumber))
                {
                    // Increment the brand number and format it as VHxxxxx
                    return "HSP" + (latestBrandNumber + 1).ToString("D5");
                }
            }

            // If no existing brands, start from VH00001
            return "HSP00001";
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
        public IActionResult Edit([FromBody] BrandModel empobj)
        {
            if (ModelState.IsValid)
            {
                empobj.ModifiedTime = DateTime.Now;

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
                return View("~/Areas/Admin/Views/Shared/_ErrorAdmin.cshtml");
            }

            try
            {
                _context.Brand.Remove(deleterecord);
                _context.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return View("~/Areas/Admin/Views/Shared/_ErrorAdmin.cshtml");

            }
        }

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

                var list = new List<BrandModel>();

                using (var stream = new MemoryStream())
                {
                    await formFile.CopyToAsync(stream, cancellationToken);

                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        var rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            var brand = new BrandModel();

                            brand.MaHang = worksheet.Cells[row, 1].Value?.ToString().Trim();
                            brand.TenHang = worksheet.Cells[row, 2].Value?.ToString().Trim();
                            brand.XuatXu = worksheet.Cells[row, 3].Value?.ToString().Trim();

                            if (DateTime.TryParse(worksheet.Cells[row, 4].Value?.ToString().Trim(), out DateTime ngaysanxuat))
                            {
                                brand.NgaySanXuat = ngaysanxuat;
                            }
                            else
                            {
                                // Handle the case where the date is invalid
                                // You can log an error, skip the record, or use a default date
                                Console.WriteLine($"Error parsing date in row {row}, column 4");
                            }

                            if (!string.IsNullOrEmpty(brand.MaHang) && !string.IsNullOrEmpty(brand.TenHang) && !string.IsNullOrEmpty(brand.XuatXu))
                            {
                                // Attempt to parse the date in the desired format
                                if (DateTime.TryParseExact(brand.NgaySanXuat.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                                {
                                    // Use the parsed date (parsedDate) for further processing
                                }
                                else
                                {
                                    // Handle the case where the date is invalid
                                    Console.WriteLine($"Error parsing date in format dd/MM/yyyy: {brand.NgaySanXuat}");
                                }
                            }
                            {
                                // Kiểm tra xem tên loại đã tồn tại chưa
                                bool isTenLoaiExists = _context.Brand.Any(u => u.TenHang == brand.TenHang);

                                if (!isTenLoaiExists)
                                {
                                    list.Add(new BrandModel
                                    {
                                        MaHang = brand.MaHang,
                                        TenHang = brand.TenHang,
                                        NgaySanXuat = brand.NgaySanXuat,
                                        XuatXu = brand.XuatXu // Assuming XuatXu is another property
                                    });
                                }
                            }
                        }
                    }
                }

                // Thêm danh sách vào cơ sở dữ liệu
                if (list.Count > 0)
                {
                    _context.Brand.AddRange(list);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View("~/Areas/Admin/Views/Shared/_ErrorAdmin.cshtml");
            }
        }



        [HttpPost]
        public IActionResult DownloadReport(IFormCollection obj)
        {
            string reportname = $"Dungcts_Brand_{Guid.NewGuid():N}.xlsx";
            var list = _IReporting.GetBrandwiseReport();
            if (list.Count > 0)
            {
                var exportbytes = _homeAdmin.ExporttoExcel<Brand_exrepoting_Dto>(list, reportname);
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
