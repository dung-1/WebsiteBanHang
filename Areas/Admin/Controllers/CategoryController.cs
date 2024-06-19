using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using WebsiteBanHang.Areas.Admin.AdminDTO;
using WebsiteBanHang.Areas.Admin.Common;
using WebsiteBanHang.Areas.Admin.Data;
using WebsiteBanHang.Areas.Admin.Models;
using WebsiteBanHang.Models;
using X.PagedList;
using static System.Web.Razor.Parser.SyntaxConstants;

namespace WebsiteBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Employee")]

    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        readonly IReporting _IReporting;
        readonly AdminHomeController _homeAdmin;

        public CategoryController(ApplicationDbContext context, IReporting iReporting, AdminHomeController homeAdmin)
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

                var sortedBrands = _context.Category.AsQueryable().OrderByDescending(b => b.Id);

                if (!string.IsNullOrEmpty(searchName))
                {
                    sortedBrands = (IOrderedQueryable<CategoryModel>)sortedBrands.Where(p => p.TenLoai.Contains(searchName));
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


                IPagedList<CategoryModel> pagedBrands = sortedProducts.ToPagedList(pageNumber, pageSize);


                if (TempData.ContainsKey("SuccessMessage"))
                {
                    ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
                }

                return View(pagedBrands);

            }
            catch (Exception ex)
            {
                return View("~/Areas/Admin/Views/Shared/_ErrorAdmin.cshtml");


            }

        }





        public IActionResult Create()
        {
            return PartialView("_CategoryCreate");
        }


        [HttpGet]
        public JsonResult IsTenLoaiExists(string tenLoai)
        {
            bool isTenLoaiExists = _context.Category.Any(u => u.TenLoai == tenLoai);
            return Json(new { exists = isTenLoaiExists });
        }

        [HttpPost]
        public IActionResult Create(CategoryModel empobj)
        {

            try
            {
                ModelState.Remove("MaLoai");

                // Kiểm tra xem tên loại đã tồn tại chưa
                bool isTenLoaiExists = _context.Category.Any(u => u.TenLoai == empobj.TenLoai);

                if (isTenLoaiExists)
                {
                    ModelState.AddModelError("TenLoai", "Tên loại sản phẩm đã tồn tại.");
                    return View(empobj);
                }

                if (ModelState.IsValid)
                {
                    // Tạo mã loại sản phẩm mới tự động và gán cho empobj.MaLoai
                    empobj.MaLoai = GenerateCategoryCode(empobj);

                    _context.Category.Add(empobj);
                    _context.SaveChanges();

                    return RedirectToAction("Index"); // Chuyển đến action "Index"
                }

                return View(empobj);
            }
            catch (Exception ex)
            {
                return View("~/Areas/Admin/Views/Shared/_ErrorAdmin.cshtml");
            }


        }

        private string GenerateCategoryCode(CategoryModel empobj)
        {
            // Get the latest category code from the database
            var latestCategory = _context.Category
                .OrderByDescending(category => category.MaLoai)
                .FirstOrDefault();

            if (latestCategory != null)
            {
                // Extract the numeric part of the latest category code
                if (int.TryParse(latestCategory.MaLoai.Substring(3), out int latestCategoryNumber))
                {
                    // Increment the category number and format it as VPxxxxx
                    return "LSP" + (latestCategoryNumber + 1).ToString("D5");
                }
            }

            // If no existing categories, start from VP00001
            return "LSP00001";
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
        public IActionResult Edit([FromBody] CategoryModel empobj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Category.Update(empobj);
                    _context.SaveChanges();
                    return RedirectToAction("Index"); // Sử dụng RedirectToAction để trả về action "Index"
                }

                return View(empobj);
            }
            catch (Exception ex)
            {
                return View("~/Areas/Admin/Views/Shared/_ErrorAdmin.cshtml");


            }

        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var deleterecord = _context.Category.Find(id);
            if (deleterecord == null)
            {
                return View("~/Areas/Admin/Views/Shared/_ErrorAdmin.cshtml");
            }

            try
            {
                _context.Category.Remove(deleterecord);
                _context.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
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

                var list = new List<CategoryModel>();

                using (var stream = new MemoryStream())
                {
                    await formFile.CopyToAsync(stream, cancellationToken);

                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        var rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            var maLoai = worksheet.Cells[row, 1].Value?.ToString().Trim();
                            var tenLoai = worksheet.Cells[row, 2].Value?.ToString().Trim();

                            if (!string.IsNullOrEmpty(maLoai) && !string.IsNullOrEmpty(tenLoai))
                            {
                                // Kiểm tra xem tên loại đã tồn tại chưa
                                bool isTenLoaiExists = _context.Category.Any(u => u.TenLoai == tenLoai);

                                if (!isTenLoaiExists)
                                {
                                    list.Add(new CategoryModel
                                    {
                                        MaLoai = maLoai,
                                        TenLoai = tenLoai
                                    });
                                }
                            }
                        }
                    }
                }

                // Thêm danh sách vào cơ sở dữ liệu
                if (list.Count > 0)
                {
                    _context.Category.AddRange(list);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                return View("~/Areas/Admin/Views/Shared/_ErrorAdmin.cshtml");


            }

        }
        //export excel
        [HttpPost]
        public IActionResult DownloadReport(IFormCollection obj)
        {
            string reportname = $"Dungcts_Category_{Guid.NewGuid():N}.xlsx";
            var list = _IReporting.GetCategorywiseReport();
            if (list.Count > 0)
            {
                var exportbytes = _homeAdmin.ExporttoExcel<Category_exrepoting_Dto>(list, reportname);
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
