using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MailKit.Net.Smtp;
using MimeKit.Text;
using WebsiteBanHang.Areas.Admin.Models;
using WebsiteBanHang.Areas.Admin.Data;
using MailKit.Security;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace WebsiteBanHang.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public AccountController(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string email, string fullname, string phoneNumber, string address, string password)
        {

            bool isEmailExists = _context.User.Any(u => u.Email == email);
            if (isEmailExists)
            {
                // Email đã tồn tại, quay lại trang đăng ký
                return RedirectToAction("Index");
            }

            // Email không trùng, lưu thông tin vào session
            Random random = new Random();
            int code = random.Next(100000, 999999);
            HttpContext.Session.SetInt32("VerificationCode", code);
            HttpContext.Session.SetString("email", email);
            HttpContext.Session.SetString("password", password);
            HttpContext.Session.SetString("fullname", fullname);
            HttpContext.Session.SetString("phoneNumber", phoneNumber);
            HttpContext.Session.SetString("address", address);

            // Gửi email chứa mã code
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Nguyễn Văn Dụng", _configuration["EmailSettings:Email"]));
            emailMessage.To.Add(new MailboxAddress("Recipient Name", email));
            emailMessage.Subject = "Xác minh tài khoản";

            emailMessage.Body = new TextPart(TextFormat.Html)
            {
                Text = $"Mã xác minh của bạn là: {code}"
            };

            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(_configuration["EmailSettings:Email"], _configuration["EmailSettings:Password"]);
            smtp.Send(emailMessage);
            smtp.Disconnect(true);

            return RedirectToAction("Check_Verification");
        }

        [HttpPost]
        public IActionResult CheckEmail(string email)
        {
            bool isEmailExists = _context.User.Any(u => u.Email == email);
            return Json(!isEmailExists);
        }
        public IActionResult Check_Verification()
        {
            // Lấy Fullname từ Session
            ViewBag.Fullname = HttpContext.Session.GetString("fullname");
            return View();
        }


        [HttpPost]
        public IActionResult Check_Verification(int code)
        {

            // Lấy mã code từ session
            int? verificationCode = HttpContext.Session.GetInt32("VerificationCode");

            if (verificationCode.HasValue && code == verificationCode.Value)
            {
                // Tạo một đối tượng UserModel từ session
                var userModel = new UserModel
                {
                    Email = HttpContext.Session.GetString("email"),
                    MatKhau = GetMd5Hash(HttpContext.Session.GetString("password")),
                    NgayTao = DateTime.Now
                };

                // Tạo một đối tượng Users_Details từ session
                var userDetails = new Users_Details
                {
                    HoTen = HttpContext.Session.GetString("fullname"),
                    SoDienThoai = int.Parse(HttpContext.Session.GetString("phoneNumber")),
                    DiaChi = HttpContext.Session.GetString("address")
                };

                // Thiết lập quan hệ giữa UserModel và Users_Details
                userModel.userDetail = userDetails;
                // Lưu thông tin vào cơ sở dữ liệu
                _context.User.Add(userModel);
                _context.SaveChanges();

                // Xóa mã code và các thông tin từ session sau khi đã sử dụng
                HttpContext.Session.Remove("VerificationCode");
                HttpContext.Session.Remove("username");
                HttpContext.Session.Remove("email");
                HttpContext.Session.Remove("password");
                HttpContext.Session.Remove("fullname");
                HttpContext.Session.Remove("phoneNumber");
                HttpContext.Session.Remove("address");
                return RedirectToAction("Login");
            }

            // Mã code không khớp, hiển thị thông báo lỗi
            return View();
        }
        public IActionResult Login()
        {
            UserModel user = new UserModel();
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserModel loginModel)
        {
            if (ModelState.IsValid)
            {
                var hashedPassword = GetMd5Hash(loginModel.MatKhau);
                var user = _context.User.FirstOrDefault(m => m.Email == loginModel.Email && m.MatKhau == hashedPassword);

                if (user != null)
                {
                    var userRoles = _context.UserRole.Where(ur => ur.User_ID == user.Id).Select(ur => ur.Role.Name).ToList();

                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email)
            };

                    foreach (var role in userRoles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }

                    // Tạo ClaimsIdentity
                    var userIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    // Tạo Principal
                    var userPrincipal = new ClaimsPrincipal(userIdentity);

                    // Đăng nhập người dùng bằng cookie
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);

                    if (userRoles.Contains("Admin"))
                    {
                        return RedirectToAction("Index", "AdminHome", new { area = "Admin" });
                    }
                    else if (userRoles.Contains("Customer"))
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            // Đăng nhập không thành công, hiển thị thông báo lỗi
            ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không chính xác.");
            return View(loginModel);
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

    }
}
