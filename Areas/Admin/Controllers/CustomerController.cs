using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
                .Where(p => p.UserRole.Any(ur => ur.Role.Name == "Customer")) 
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

    }
}
