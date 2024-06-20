using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Web.Mvc;
using WebsiteBanHang.Areas.Admin.AdminDTO;
using WebsiteBanHang.Areas.Admin.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace WebsiteBanHang.ViewComponents
{
    [Authorize(Roles = "Customer")]
    public class ApprovedViewComponent : ViewComponent
    {
        private readonly ILogger<ApprovedViewComponent> _logger;
        private readonly ApplicationDbContext _context;

        public ApprovedViewComponent(ApplicationDbContext context, ILogger<ApprovedViewComponent> logger)
        {
            _context = context;
            _logger = logger;
        }

        private async Task<IViewComponentResult> GetOrdersByStatusAsync(int? page, string searchName, string orderStatus)
        {
            try
            {
                // Lấy thông tin khách hàng đang đăng nhập từ HttpContext
                var loggedInCustomerClaim = HttpContext.User.FindFirst(ClaimTypes.Name);
                if (loggedInCustomerClaim != null)
                {
                    string loggedInCustomerEmail = loggedInCustomerClaim.Value;

                    // Tìm khách hàng theo email
                    var loggedInCustomer = await _context.Customer
                        .Include(c => c.CustomerDetail)
                        .FirstOrDefaultAsync(c => c.Email == loggedInCustomerEmail);

                    if (loggedInCustomer != null)
                    {
                        var pageNumber = page ?? 1;
                        int pageSize = 10;

                        // Lấy danh sách đơn hàng của khách hàng đăng nhập
                        var productsQuery = _context.Order
                            .Where(o => o.CustomerID == loggedInCustomer.Id && o.trangThai == orderStatus)
                            .OrderByDescending(o => o.id)
                            .Select(o => new
                            {
                                Order = o,
                                CustomerDetail = o.Customer.CustomerDetail
                            });

                        // Áp dụng bộ lọc theo tên đơn hàng nếu có
                        if (!string.IsNullOrEmpty(searchName))
                        {
                            productsQuery = productsQuery.Where(p => p.Order.trangThai.Contains(searchName));
                        }

                        var sortedProducts = await productsQuery.ToListAsync();

                        // Chuyển đổi danh sách sang đối tượng PagedList
                        var pagedCategories = sortedProducts.Select(e => new OrderDto
                        {
                            Id = e.Order.id,
                            MaHoaDon = e.Order.MaHoaDon,
                            TenNhanVien = e.CustomerDetail != null ? e.CustomerDetail.HoTen : null,
                            NgayBan = e.Order.ngayBan,
                            LoaiHoaDon = e.Order.LoaiHoaDon,
                            TrangThai = e.Order.trangThai,
                        }).ToPagedList(pageNumber, pageSize);
                        ViewBag.SearchName = searchName ?? "";

                        if (TempData["SuccessMessage"] != null)
                        {
                            ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
                        }

                        return View(pagedCategories);
                    }
                }

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting orders by status.");
                return View();
            }
        }

        public Task<IViewComponentResult> InvokeAsync(int? page, string searchName)
        {
            return GetOrdersByStatusAsync(page, searchName, "Đã duyệt");
        }
    }
}