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
                return Json(new { success = false, message = "Người dùng chưa đăng nhập hoặc ID không hợp lệ." });
            }

            if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(newPasswordAgain))
            {
                return Json(new { success = false, message = "Vui lòng nhập đầy đủ thông tin." });
            }

            if (newPassword != newPasswordAgain)
            {
                return Json(new { success = false, message = "Mật khẩu mới không khớp." });
            }

            var customer = await _context.Customer.FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
            {
                return Json(new { success = false, message = "Không tìm thấy khách hàng." });
            }

            var currentPasswordHash = GetMd5Hash(currentPassword);
            if (customer.MatKhau != currentPasswordHash)
            {
                return Json(new { success = false, message = "Mật khẩu hiện tại không đúng." });
            }

            customer.MatKhau = GetMd5Hash(newPassword);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Cập nhật mật khẩu thành công." });
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
