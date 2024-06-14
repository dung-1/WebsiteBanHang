using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Areas.Admin.Data;
using WebsiteBanHang.Areas.Admin.Models;
using X.PagedList;

namespace WebsiteBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Employee")]

    public class BrandController : Controller
    {
        private readonly ApplicationDbContext _context;
        public BrandController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? page, string searchName)
        {
            try
            {
                var pageNumber = page ?? 1; // Số trang mặc định (trang 1)
                int pageSize = 5; // Số mục trên mỗi trang

                var sortedBrands = _context.Brand.AsQueryable().OrderByDescending(b => b.Id);

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
    }

}
