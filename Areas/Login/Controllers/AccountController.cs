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
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace WebsiteBanHang.Controllers
{
    [Area("Login")]
    [Route("Login/account")]
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public AccountController(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
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

    }
}
