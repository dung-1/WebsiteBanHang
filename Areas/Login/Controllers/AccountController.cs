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
using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Areas.Admin.AdminDTO;
using WebsiteBanHang.Areas.Admin.Controllers;

namespace WebsiteBanHang.Controllers
{
    [Area("Login")]
    [Route("Login/account")]
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BillorderController> _logger;

        public AccountController(IConfiguration configuration, ApplicationDbContext context, ILogger<BillorderController> logger)
        {
            _configuration = configuration;
            _context = context;
            _logger = logger;
        }
        [Route("account")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("formSignUp")]
        public IActionResult formSignUp(string email, string fullname, string phoneNumber, string address, string password)
        {
            string userCodeString = "KH00001"; // Giá trị mặc định

            var lastUser = _context.Customer.OrderByDescending(u => u.Id).FirstOrDefault();

            if (lastUser != null)
            {
                // Lấy mã người dùng cuối cùng, tăng giá trị lên 1 và sử dụng cho người dùng mới
                string lastUserCode = lastUser.MaNguoiDung;
                int lastUserCodeNumber = int.Parse(lastUserCode.Substring(4)); // Loại bỏ "user" và chuyển thành số
                int newCodeNumber = lastUserCodeNumber + 1;
                userCodeString = "KH" + newCodeNumber.ToString("D5");
            }

            bool isEmailExists = _context.Customer.Any(u => u.Email == email);
            if (isEmailExists)
            {
                // Email đã tồn tại, quay lại trang đăng ký
                return RedirectToAction("Index");
            }

            // Email không trùng, lưu thông tin vào session
            Random random = new Random();
            int code = random.Next(100000, 999999);
            HttpContext.Session.SetInt32("VerificationCode", code);// Tạo mã người dùng mới
            HttpContext.Session.SetString("iserCode", userCodeString);
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
        [Route("CheckEmail")]
        public IActionResult CheckEmail(string email)
        {
            bool isEmailExists = _context.Customer.Any(u => u.Email == email);
            return Json(!isEmailExists);
        }


        [Route("Check_Verification")]
        public IActionResult Check_Verification()
        {
            // Lấy Fullname từ Session
            ViewBag.Fullname = HttpContext.Session.GetString("fullname");
            return View();
        }


        [HttpPost]
        [Route("formCheck_Verification")]
        public IActionResult formCheck_Verification(int code)
        {
            int? verificationCode = HttpContext.Session.GetInt32("VerificationCode");
            string? customerCodeString = HttpContext.Session.GetString("iserCode");

            if (verificationCode.HasValue && code == verificationCode.Value)
            {
                // Tạo một đối tượng UserModel từ session
                var CustomerModel = new CustomerModel
                {
                    MaNguoiDung = customerCodeString, // Sử dụng mã người dùng từ session
                    Email = HttpContext.Session.GetString("email"),
                    MatKhau = GetMd5Hash(HttpContext.Session.GetString("password")),
                    NgayTao = DateTime.Now
                };

                // Tạo một đối tượng Users_Details từ session
                var CustomerDetails = new Customer_Details
                {
                    HoTen = HttpContext.Session.GetString("fullname"),
                    SoDienThoai = HttpContext.Session.GetString("phoneNumber"),
                    DiaChi = HttpContext.Session.GetString("address")
                };

                // Thiết lập quan hệ giữa UserModel và Users_Details
                CustomerModel.CustomerDetail = CustomerDetails;

                // Lưu thông tin người dùng vào cơ sở dữ liệu
                _context.Customer.Add(CustomerModel);
                _context.SaveChanges();

                // Tạo một bản ghi trong bảng UserRole để gán người dùng vào vai trò "Customer" (hoặc vai trò tương ứng)
                var customerRoleId = _context.Role.FirstOrDefault(r => r.Name == "Customer")?.Id;
                if (customerRoleId.HasValue)
                {
                    var CustomerRole = new CustomerRoleModel
                    {
                        Customer_ID = CustomerModel.Id, // ID của người dùng mới
                        Role_ID = customerRoleId.Value // ID của vai trò "Customer" hoặc tương ứng
                    };
                    _context.CustomerRole.Add(CustomerRole);
                    _context.SaveChanges();
                }

                // Xóa mã code và các thông tin từ session sau khi đã sử dụng
                HttpContext.Session.Remove("VerificationCode");
                HttpContext.Session.Remove("iserCode");
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

        [Route("Login")]
        public IActionResult Login()
        {
            UserModel user = new UserModel();
            return View(user);
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(UserModel loginModel)
        {
            var hashedPassword = GetMd5Hash(loginModel.MatKhau);

            // Check in the User table
            var user = _context.User
                .Include(u => u.userDetail) // Include the navigation property
                .FirstOrDefault(m => m.Email == loginModel.Email && m.MatKhau == hashedPassword);

            if (user == null)
            {
                // Check in the Customer table
                var customer = _context.Customer
                    .Include(c => c.CustomerDetail) // Include the navigation property
                    .FirstOrDefault(m => m.Email == loginModel.Email && m.MatKhau == hashedPassword);

                if (customer != null)
                {
                    var customerRoles = _context.CustomerRole
                        .Include(cr => cr.Role)
                        .Where(cr => cr.Customer_ID == customer.Id)
                        .Select(cr => cr.Role.Name)
                        .ToList();

                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, customer.Email),
                new Claim(ClaimTypes.Email, customer.CustomerDetail?.HoTen),
                new Claim(ClaimTypes.StreetAddress, customer.CustomerDetail?.DiaChi),
                new Claim(ClaimTypes.MobilePhone, customer.CustomerDetail?.SoDienThoai),
                new Claim("UserId", customer.Id.ToString()) // Add UserId claim
            };

                    foreach (var role in customerRoles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }

                    var userIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var userPrincipal = new ClaimsPrincipal(userIdentity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);

                    ViewBag.UserId = customer.Id; // Set UserId in ViewBag

                    if (customerRoles.Contains("Admin") || customerRoles.Contains("Employee"))
                    {
                        return RedirectToAction("Index", "AdminHome", new { area = "Admin" });
                    }
                    else if (customerRoles.Contains("Customer"))
                    {
                        return RedirectToAction("Index", "User", new { area = "" });
                    }
                }
            }
            else
            {
                var userRoles = _context.UserRole
                    .Include(ur => ur.Role)
                    .Where(ur => ur.User_ID == user.Id)
                    .Select(ur => ur.Role.Name)
                    .ToList();

                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Email, user.userDetail?.HoTen), // Assuming userDetail is a navigation property
            new Claim("UserId", user.Id.ToString()) // Add UserId claim
        };

                foreach (var role in userRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var userIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var userPrincipal = new ClaimsPrincipal(userIdentity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);

                ViewBag.AdminId = user.Id; // Set UserId in ViewBag

                if (userRoles.Contains("Admin") || userRoles.Contains("Employee"))
                {
                    return RedirectToAction("Index", "AdminHome", new { area = "Admin" });
                }
                else if (userRoles.Contains("Customer"))
                {
                    return RedirectToAction("Index", "User");
                }
            }

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

        [HttpGet("forgot-password")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            try
            {
                var users = await _context.Customer
                    .Where(u => u.Email == email)
                    .ToListAsync();

                if (users.Count == 0)
                    return Json(new { success = false, message = "Không tìm thấy người dùng với email này" });

                foreach (var user in users)
                {
                    user.ResetPasswordToken = Guid.NewGuid().ToString();
                    user.ResetPasswordTokenExpiry = DateTime.UtcNow.AddMinutes(30);
                }

                await _context.SaveChangesAsync();

                foreach (var user in users)
                {
                    var resetLink = Url.Action("ResetPassword", "Account", new { token = user.ResetPasswordToken }, Request.Scheme);

                    var emailMessage = new MimeMessage();
                    emailMessage.From.Add(new MailboxAddress("Nguyễn Văn Dụng", _configuration["EmailSettings:Email"]));
                    emailMessage.To.Add(new MailboxAddress(user.Email, user.Email));
                    emailMessage.Subject = "Đặt lại mật khẩu của bạn";
                    emailMessage.Body = new TextPart(TextFormat.Html)
                    {
                        Text = $@"
            <h2>Xin chào {user.Email},</h2>
            <p>Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn.</p>
            <p>Click <a href='{resetLink}'>vào đây</a> để đặt lại mật khẩu của bạn.</p>
            <p>Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này.</p>
            <p>Trân trọng,<br>Đội ngũ hỗ trợ</p>"
                    };

                    try
                    {
                        using var smtp = new SmtpClient();
                        await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                        await smtp.AuthenticateAsync(_configuration["EmailSettings:Email"], _configuration["EmailSettings:Password"]);
                        await smtp.SendAsync(emailMessage);
                        await smtp.DisconnectAsync(true);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Lỗi khi gửi email đến {user.Email}: {ex.Message}");
                        continue;
                    }
                }

                return Json(new { success = true, message = "Liên kết đặt lại mật khẩu đã được gửi đến email của bạn." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi trong quá trình xử lý ForgotPassword: {ex.Message}");
                return Json(new { success = false, message = "Đã xảy ra lỗi trong quá trình xử lý yêu cầu." });
            }
        }


        [HttpGet("reset-password")]
        public IActionResult ResetPassword(string token)
        {
            var model = new ResetPasswordDto { Token = token };
            return View(model);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Token) || string.IsNullOrEmpty(model.NewPassword))
                {
                    return Json(new { success = false, message = "Token và mật khẩu mới không được để trống" });
                }

                var user = await _context.Customer
                    .FirstOrDefaultAsync(u => u.ResetPasswordToken == model.Token && u.ResetPasswordTokenExpiry > DateTime.UtcNow);

                if (user == null)
                {
                    return Json(new { success = false, message = "Token không hợp lệ hoặc đã hết hạn" });

                }

                // Kiểm tra độ phức tạp của mật khẩu
                if (!IsPasswordComplex(model.NewPassword))
                {
                    return Json(new { success = false, message = "Mật khẩu mới không đủ phức tạp. Vui lòng sử dụng ít nhất 8 ký tự, bao gồm chữ hoa, chữ thường, số và ký tự đặc biệt." });
                }

                // Cập nhật mật khẩu mới với mã hóa MD5
                user.MatKhau = GetMd5Hash(model.NewPassword);
                user.ResetPasswordToken = null;
                user.ResetPasswordTokenExpiry = null;

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Mật khẩu của bạn đã được đặt lại thành công." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi trong quá trình xử lý đổi mật khẩu: {ex.Message}");
                return Json(new { success = false, message = "Đã xảy ra lỗi trong quá trình xử lý yêu cầu." });
            }
        }

        private bool IsPasswordComplex(string password)
        {
            // Kiểm tra độ dài tối thiểu
            if (password.Length < 8) return false;

            // Kiểm tra có chữ hoa
            if (!password.Any(char.IsUpper)) return false;

            // Kiểm tra có chữ thường
            if (!password.Any(char.IsLower)) return false;

            // Kiểm tra có số
            if (!password.Any(char.IsDigit)) return false;

            // Kiểm tra có ký tự đặc biệt
            if (!password.Any(ch => !char.IsLetterOrDigit(ch))) return false;

            return true;
        }

    }
}
