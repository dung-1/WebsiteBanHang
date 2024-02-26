using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using WebsiteBanHang.Areas.Admin.AdminDTO;
using WebsiteBanHang.Areas.Admin.Data;
using WebsiteBanHang.Areas.Admin.Models;
using X.PagedList;

namespace WebsiteBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Employee")]

    public class InventoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        public InventoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? page, string searchName)
        {
            var pageNumber = page ?? 1; // Số trang mặc định (trang 1)
            int pageSize = 5; // Số mục trên mỗi trang


            var productsQuery = _context.Inventory
                .Include(p => p.product)
                .OrderByDescending(p => p.Id);

            if (!string.IsNullOrEmpty(searchName))
            {
                productsQuery = (IOrderedQueryable<InventoriesModel>)productsQuery.Where(p => p.product.TenSanPham.Contains(searchName));
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


            if (TempData.ContainsKey("SuccessMessage"))
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }
            IPagedList<InventoryViewDto> pagedProducts = sortedProducts
                           .Select(e => new InventoryViewDto
                           {
                               Id = e.Id,
                               TenSanPham = e.product.TenSanPham,
                               MaKho=e.MaKho,
                               NgayNhap = e.NgayNhap,
                               SoLuong = e.SoLuong,
                           })
                           .ToPagedList(pageNumber, pageSize);

            return View(pagedProducts);
        }

        public IActionResult Create()
        {
            // Truy vấn danh sách sản phẩm chưa được thêm vào kho
            var chuaThemVaoKhoList = _context.Product.Where(p => p.Inventory.All(i => i.SoLuong == 0)).ToList();

            // Truy vấn danh sách loại sản phẩm
            var loaiProductList = chuaThemVaoKhoList.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.TenSanPham
            }).ToList();

            ViewBag.LoaiProductList = loaiProductList;
            return PartialView("_InventoryCreate");
        }

        [HttpPost]
        public IActionResult Create(InventoriesModel empobj)
        {
            // Tạo mã loại sản phẩm mới tự động và gán cho empobj.MaLoai
            empobj.MaKho = GenerateCategoryCode(empobj);
            var product = _context.Product.Find(empobj.ProductId);
            if (product != null)
            {
                empobj.product = product;
                _context.Inventory.Add(empobj);
                _context.SaveChanges();

                return RedirectToAction("Index"); // Chuyển đến action "Index"

            }
            return View(empobj);
        }

        private string GenerateCategoryCode(InventoriesModel empobj)
        {
            // Get the latest category code from the database
            var latestCategory = _context.Inventory
                .OrderByDescending(category => category.MaKho)
                .FirstOrDefault();

            if (latestCategory != null)
            {
                // Extract the numeric part of the latest category code
                if (int.TryParse(latestCategory.MaKho.Substring(3), out int latestCategoryNumber))
                {
                    // Increment the category number and format it as VPxxxxx
                    return "KSP" + (latestCategoryNumber + 1).ToString("D5");
                }
            }

            // If no existing categories, start from VP00001
            return "KSP00001";
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            // Truy vấn thông tin chi tiết của sản phẩm cần chỉnh sửa
            var inventory = _context.Inventory.Include(i => i.product).FirstOrDefault(i => i.Id == id);

            if (inventory == null)
            {
                return NotFound();
            }

            // Tạo một SelectListItem với thông tin sản phẩm cần chỉnh sửa
            var selectedItem = new SelectListItem
            {
                Value = inventory.ProductId.ToString(),
                Text = inventory.product.TenSanPham // Lấy tên sản phẩm từ navigation property Product
            };

            // Gán selectedItem vào ViewBag.LoaiProductList
            ViewBag.LoaiProductList = new SelectList(new List<SelectListItem> { selectedItem }, "Value", "Text");

            return PartialView("_InventoryEdit", inventory);
        }

        [HttpPost]
        public IActionResult Edit([FromBody] InventoriesModel empobj)
        {
            var brand = _context.Product.Find(empobj.ProductId);

            if (brand != null)
            {

                empobj.product = brand;
                _context.Inventory.Update(empobj);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(empobj);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var deleterecord = _context.Inventory.Find(id);
            if (deleterecord == null)
            {
                return NotFound();
            }
            try
            {
                _context.Inventory.Remove(deleterecord);
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
