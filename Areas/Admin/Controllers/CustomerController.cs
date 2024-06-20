using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using WebsiteBanHang.Areas.Admin.AdminDTO;
using WebsiteBanHang.Areas.Admin.Data;
using WebsiteBanHang.Areas.Admin.Models;
using X.PagedList;
using static WebsiteBanHang.Areas.Admin.Data.ApplicationDbContext;

namespace WebsiteBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Employee")]
    public class CustomerController : Controller
    {
        private readonly ILogger<CustomerController> _logger;

        private readonly ApplicationDbContext _context;
        public CustomerController(ApplicationDbContext context, ILogger<CustomerController> logger)
        {
            _context = context;
            _logger = logger;

        }

        public IActionResult Index(int? page, string searchName)
        {
            try
            {
                var pageNumber = page ?? 1;
                int pageSize = int.MaxValue; // Số mục trên mỗi trang

                var productsQuery = _context.Customer
                    .Include(p => p.CustomerRole)
                    .ThenInclude(ur => ur.Role) // Include Role information
                    .Include(p => p.CustomerDetail)
                    .OrderByDescending(p => p.Id);

                if (!string.IsNullOrEmpty(searchName))
                {
                    productsQuery = (IOrderedQueryable<CustomerModel>)productsQuery.Where(p => p.CustomerDetail.HoTen.Contains(searchName));
                }

                var sortedProducts = productsQuery
                    .Where(p => p.CustomerRole.Any(ur => ur.Role.Name == "Customer"))
                    .ToList();

                if (searchName != null)
                {
                    ViewBag.SearchName = searchName;
                }
                else
                {
                    ViewBag.SearchName = ""; // Hoặc gán một giá trị mặc định khác nếu cần thiết
                }

                IPagedList<UserModelViewDto> pagedProducts = sortedProducts
                    .Select(e => new UserModelViewDto
                    {
                        Id = e.Id,
                        MaNguoiDung = e.MaNguoiDung,
                        HoTen = e.CustomerDetail.HoTen,
                        Email = e.Email,
                    })
                    .ToPagedList(pageNumber, pageSize);

                return View(pagedProducts);
            }
            catch (Exception e)
            {
                return View("~/Areas/Admin/Views/Shared/_ErrorAdmin.cshtml");
            }

        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var deleterecord = _context.Customer.Find(id);
            if (deleterecord == null)
            {
                return NotFound();
            }

            try
            {
                _context.Customer.Remove(deleterecord);
                _context.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return View("~/Areas/Admin/Views/Shared/_ErrorAdmin.cshtml");
            }
        }



        [HttpGet]
        public IActionResult Edit(int id)
        {
            var user = _context.Customer
                .Include(u => u.CustomerDetail)
                .FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }
            var userDto = new UserCreateDto
            {
                Id = user.Id,
                Email = user.Email,
                HoTen = user.CustomerDetail?.HoTen,
                SoDienThoai = user.CustomerDetail?.SoDienThoai,
                DiaChi = user.CustomerDetail?.DiaChi,
            };

            return PartialView("_CustomerEdit", userDto);
        }
        private string GetMd5Hash(string input)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    builder.Append(data[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        [HttpPost]
        public IActionResult Edit([FromBody] UserCreateDto userDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        // Lấy người dùng từ cơ sở dữ liệu
                        var customer = _context.Customer
                            .Include(u => u.CustomerDetail)
                            .Include(u => u.CustomerRole)
                            .FirstOrDefault(u => u.Id == userDto.Id);

                        if (customer == null)
                        {
                            return NotFound();
                        }

                        // Cập nhật thông tin từ userDto vào user
                        customer.Email = userDto.Email;
                        customer.NgayTao = DateTime.Now;
                        customer.CustomerDetail.HoTen = userDto.HoTen;
                        customer.CustomerDetail.SoDienThoai = userDto.SoDienThoai;
                        customer.CustomerDetail.DiaChi = userDto.DiaChi;

                        // Nếu có mật khẩu mới được nhập, hãy cập nhật nó
                        if (!string.IsNullOrEmpty(userDto.MatKhau))
                        {
                            // Ở đây, bạn thường sẽ mã hóa mật khẩu mới trước khi lưu vào cơ sở dữ liệu
                            // Ví dụ: user.MatKhau = Mã hóa(userDto.MatKhau);
                            customer.MatKhau = GetMd5Hash(userDto.MatKhau);
                        }



                        // Lưu thay đổi vào cơ sở dữ liệu
                        _context.SaveChanges();

                        return RedirectToAction("Index"); // Chuyển hướng đến trang danh sách người dùng sau khi chỉnh sửa thành công
                    }
                    catch (Exception)
                    {
                        return View("~/Areas/Admin/Views/Shared/_ErrorAdmin.cshtml");
                    }
                }

                // Nếu ModelState không hợp lệ, quay trở lại view chỉnh sửa với dữ liệu và thông báo lỗi
                var roleList = _context.Role.ToList();
                roleList = roleList.Where(role => role.Name != "Customer").ToList();
                ViewBag.Vatrolist = new SelectList(roleList, "Id", "Name", userDto.VaiTroId);

                return View("_CustomerEdit", userDto);
            }
            catch (Exception ex)
            {

                return View("~/Areas/Admin/Views/Shared/_ErrorAdmin.cshtml");
            }

        }


    }
}
