using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebsiteBanHang.Areas.Admin.Data;
using WebsiteBanHang.Areas.Admin.Models;

namespace WebsiteBanHang.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerInfoController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        public CustomerInfoController(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        public IActionResult AccountInfo()
        {
            return View();
        }
        public IActionResult Changepassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string newPasswordAgain)
        {
            var customerId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(customerId) || !int.TryParse(customerId, out int id))
            {
                return Unauthorized("Người dùng chưa đăng nhập hoặc ID không hợp lệ.");
            }

            if (newPassword != newPasswordAgain)
            {
                ModelState.AddModelError("", "Mật khẩu mới không khớp.");
                return View();
            }

            var customer = await _context.Customer
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
            {
                return NotFound("Không tìm thấy khách hàng.");
            }

            var currentPasswordHash = GetMd5Hash(currentPassword);
            if (customer.MatKhau != currentPasswordHash)
            {
                ModelState.AddModelError("", "Mật khẩu hiện tại không đúng.");
                return View();
            }

            customer.MatKhau = GetMd5Hash(newPassword);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "User"); // Hoặc trang bạn muốn chuyển hướng sau khi đổi mật khẩu thành công
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
        public IActionResult Setting()
        {
            return View();
        }


       
    }
}
