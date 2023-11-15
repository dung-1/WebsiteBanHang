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

    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;

        private readonly ApplicationDbContext _context;
        public UserController(ApplicationDbContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;

        }

        public IActionResult Index(int? page, string searchName)
        {
            var pageNumber = page ?? 1;
            int pageSize = 5;

            var productsQuery = _context.User
                .Include(p => p.UserRole)
                .ThenInclude(ur => ur.Role) // Include Role information
                .Include(p => p.userDetail)
                .OrderByDescending(p => p.Id);

            if (!string.IsNullOrEmpty(searchName))
            {
                productsQuery = (IOrderedQueryable<UserModel>)productsQuery.Where(p => p.userDetail.HoTen.Contains(searchName));
            }

            var sortedProducts = productsQuery
                .Where(p => !p.UserRole.Any(ur => ur.Role.Name == "Customer")) // Loại bỏ người dùng có vai trò "Customer"
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
                    HoTen = e.userDetail.HoTen,
                    Email = e.Email,
                    VaiTro = e.UserRole
                        .Select(ur => ur.Role.Name)
                        .FirstOrDefault()
                })
                .ToPagedList(pageNumber, pageSize);

            return View(pagedProducts);
        }
        public IActionResult Create()
        {
            // Truy vấn danh sách loại sản phẩm và hãng sản phẩm từ cơ sở dữ liệu
            var roleList = _context.Role.ToList();

            // Lọc bỏ vai trò "Customer" (hoặc các vai trò khác mà bạn muốn loại bỏ)
            roleList = roleList.Where(role => role.Name != "Customer").ToList();

            // Tạo SelectList để sử dụng trong dropdown
            ViewBag.Vatrolist = new SelectList(roleList, "Id", "Name");

            return PartialView("_UserCreate");
        }


        [HttpGet]
        public JsonResult IsEmailExists(string Email)
        {
            bool isEmailExists = _context.User.Any(p => p.Email == Email);
            return Json(new { exists = isEmailExists });
        }


        [HttpPost]
        public IActionResult Create(UserCreateDto userCreateDto)
        {

            bool IsEmailExists = _context.User.Any(u => u.Email == userCreateDto.Email);

            if (IsEmailExists)
            {
                ModelState.AddModelError("Email", "Email đã tồn tại.");
                return View(userCreateDto);
            }


            if (ModelState.IsValid)
            {

                string userCodeString = "user00001"; // Giá trị mặc định

                var lastUser = _context.User.OrderByDescending(u => u.Id).FirstOrDefault();

                if (lastUser != null)
                {
                    // Lấy mã người dùng cuối cùng, tăng giá trị lên 1 và sử dụng cho người dùng mới
                    string lastUserCode = lastUser.MaNguoiDung;
                    int lastUserCodeNumber = int.Parse(lastUserCode.Substring(4)); // Loại bỏ "user" và chuyển thành số
                    int newCodeNumber = lastUserCodeNumber + 1;
                    userCodeString = "user" + newCodeNumber.ToString("D5");
                }
                // Tạo một đối tượng UserModel từ UserCreateDto
                var userModel = new UserModel
                {
                    MaNguoiDung = userCodeString,
                    Email = userCreateDto.Email,
                    MatKhau = GetMd5Hash(userCreateDto.MatKhau),
                    NgayTao = DateTime.Now,
                };

                // Tạo một đối tượng Users_Details từ UserCreateDto
                var userDetail = new Users_Details
                {
                    HoTen = userCreateDto.HoTen,
                    SoDienThoai = userCreateDto.SoDienThoai,
                    DiaChi = userCreateDto.DiaChi,
                };

                // Thêm đối tượng Users_Details vào UserModel
                userModel.userDetail = userDetail;

                // Tạo một đối tượng UserRoleModel từ UserCreateDto (VaiTroId)
                var roleModel = new UserRoleModel
                {
                    Role_ID = (int)userCreateDto.VaiTroId, // Sử dụng VaiTroId từ UserCreateDto
                };

                // Thêm UserRoleModel vào UserModel
                userModel.UserRole.Add(roleModel);

                // Thêm UserModel vào cơ sở dữ liệu
                _context.User.Add(userModel);
                _context.SaveChanges();

                return RedirectToAction("Index"); // Chuyển hướng sau khi tạo thành công
            }

            // Nếu ModelState không hợp lệ, trả về lại view với thông báo lỗi
            return View(userCreateDto);
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
        public IActionResult Delete(int id)
        {
            var deleterecord = _context.User.Find(id);
            if (deleterecord == null)
            {
                return NotFound();
            }

            try
            {
                _context.User.Remove(deleterecord);
                _context.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Log lỗi hoặc xử lý nếu cần
                return Json(new { success = false });
            }
        }




        [HttpGet]
        public IActionResult Edit(int id)
        {
            var user = _context.User
                .Include(u => u.userDetail)
                .Include(u => u.UserRole)
                .FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }
            var roleList = _context.Role.ToList();
            roleList = roleList.Where(role => role.Name != "Customer").ToList();

            // Tạo SelectList và thiết lập selected item
            ViewBag.Vatrolist = new SelectList(roleList, "Id", "Name", user.UserRole.Select(ur => ur.Role_ID).ToList());

            var userDto = new UserCreateDto
            {
                Id = user.Id,
                Email = user.Email,
                HoTen = user.userDetail?.HoTen,
                SoDienThoai = user.userDetail?.SoDienThoai,
                DiaChi = user.userDetail?.DiaChi,
                VaiTroId = user.UserRole.Select(ur => ur.Role_ID).FirstOrDefault()
            };

            return PartialView("_UserEdit", userDto);
        }

        [HttpPost]
        public IActionResult Edit([FromBody]UserCreateDto userDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Lấy người dùng từ cơ sở dữ liệu
                    var user = _context.User
                        .Include(u => u.userDetail)
                        .Include(u => u.UserRole)
                        .FirstOrDefault(u => u.Id == userDto.Id);

                    if (user == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật thông tin từ userDto vào user
                    user.Email = userDto.Email;
                    user.NgayTao = DateTime.Now;
                    user.userDetail.HoTen = userDto.HoTen;
                    user.userDetail.SoDienThoai = userDto.SoDienThoai;
                    user.userDetail.DiaChi = userDto.DiaChi;

                    // Nếu có mật khẩu mới được nhập, hãy cập nhật nó
                    if (!string.IsNullOrEmpty(userDto.MatKhau))
                    {
                        // Ở đây, bạn thường sẽ mã hóa mật khẩu mới trước khi lưu vào cơ sở dữ liệu
                        // Ví dụ: user.MatKhau = Mã hóa(userDto.MatKhau);
                        user.MatKhau = GetMd5Hash(userDto.MatKhau);
                    }

                    // Cập nhật vai trò
                    var selectedRoleId = userDto.VaiTroId;
                    var existingUserRoleIds = user.UserRole.Select(ur => ur.Role_ID).ToList();

                    // Xóa các vai trò không được chọn
                    foreach (var roleId in existingUserRoleIds)
                    {
                        if (!selectedRoleId.HasValue || roleId != selectedRoleId.Value)
                        {
                            var userRoleToRemove = user.UserRole.FirstOrDefault(ur => ur.Role_ID == roleId);
                            if (userRoleToRemove != null)
                            {
                                _context.UserRole.Remove(userRoleToRemove);
                            }
                        }
                    }

                    // Nếu có vai trò mới được chọn, thêm vào
                    if (selectedRoleId.HasValue && !existingUserRoleIds.Contains(selectedRoleId.Value))
                    {
                        var newUserRole = new UserRoleModel
                        {
                            User_ID = user.Id,
                            Role_ID = selectedRoleId.Value
                        };
                        _context.UserRole.Add(newUserRole);
                    }

                    // Lưu thay đổi vào cơ sở dữ liệu
                    _context.SaveChanges();

                    return RedirectToAction("Index"); // Chuyển hướng đến trang danh sách người dùng sau khi chỉnh sửa thành công
                }
                catch (Exception )
                {
                    // Xử lý lỗi nếu có
                    ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi cập nhật người dùng.");
                }
            }

            // Nếu ModelState không hợp lệ, quay trở lại view chỉnh sửa với dữ liệu và thông báo lỗi
            var roleList = _context.Role.ToList();
            roleList = roleList.Where(role => role.Name != "Customer").ToList();
            ViewBag.Vatrolist = new SelectList(roleList, "Id", "Name", userDto.VaiTroId);

            return View("_UserEdit", userDto);
        }




    }

}
