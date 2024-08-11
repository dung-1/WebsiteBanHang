using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebsiteBanHang.Areas.Admin.AdminDTO;
using WebsiteBanHang.Areas.Admin.Controllers;
using WebsiteBanHang.Areas.Admin.Data;
using X.PagedList;

namespace WebsiteBanHang.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerOrderController : Controller
    {
        private readonly ILogger<CustomerOrderController> _logger;

        private readonly ApplicationDbContext _context;
        public CustomerOrderController(ApplicationDbContext context, ILogger<CustomerOrderController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index(int? page, string searchName)
        {
            return GetOrdersByStatus(page, searchName, "Đang xử lý");
        }

        public IActionResult Approved(int? page, string searchName)
        {
            return GetOrdersByStatus(page, searchName, "Đã duyệt");
        }
        public IActionResult Transport(int? page, string searchName)
        {
            return GetOrdersByStatus(page, searchName, "Đang giao hàng");
        }

        public IActionResult Complete(int? page, string searchName)
        {
            return GetOrdersByStatus(page, searchName, "Hoàn thành");
        }

        public IActionResult CancelOrders(int? page, string searchName)
        {
            return GetOrdersByStatus(page, searchName, "Đã hủy");
        }

        public IActionResult OrderDetail(int id)
        {
            var orderDetailsQuery = from order in _context.Order
                                    join customerDetail in _context.Customer_Details
                                    on order.CustomerID equals customerDetail.CustomerId into orderCustomerDetails
                                    from customer in orderCustomerDetails.DefaultIfEmpty()
                                    join userDetail in _context.Users_Details
                                    on order.UserID equals userDetail.UserId into orderUserDetails
                                    from user in orderUserDetails.DefaultIfEmpty()
                                    join orderDetail in _context.Order_Detai
                                    on order.id equals orderDetail.OrderId into orderDetails
                                    from details in orderDetails.DefaultIfEmpty()
                                    join product in _context.Product
                                    on details.ProductId equals product.Id into orderProductDetails
                                    from orderProduct in orderProductDetails.DefaultIfEmpty()
                                    where order.id == id
                                    orderby order.id descending
                                    select new
                                    {
                                        Order = order,
                                        UserDetail = user,
                                        CustomerDetail = customer,
                                        OrderDetails = details,
                                        Product = orderProduct
                                    };

            var orderDetailsList = orderDetailsQuery.ToList();

            if (orderDetailsList.Count == 0)
            {
                return NotFound();
            }

            var distinctOrders = orderDetailsList
                .GroupBy(o => o.Order.id)
                .Select(group => group.First())
                .ToList();

            var orderDtos = distinctOrders.Select(orderInfo => new OrderDto
            {
                Id = orderInfo.Order.id,
                MaHoaDon = orderInfo.Order.MaHoaDon,
                TenKhachHang = orderInfo.CustomerDetail?.HoTen,
                TenNhanVien = orderInfo.UserDetail?.HoTen,
                SoDienThoai = orderInfo.CustomerDetail?.SoDienThoai,
                DiaChi = orderInfo.CustomerDetail?.DiaChi,
                NgayBan = orderInfo.Order.ngayBan,
                TrangThai = orderInfo.Order.trangThai,
                LoaiHoaDon = orderInfo.Order.LoaiHoaDon,
                ChiTietHoaDon = orderDetailsList
                .Where(o => o.Order.id == orderInfo.Order.id)
                .Select(o => new ChiTietHoaDonDto
                {
                    img = o.OrderDetails.product.Image,
                    TenSanPham = o.OrderDetails.product.TenSanPham,
                    SoLuong = o.OrderDetails.soLuong,
                    Gia = o.OrderDetails.product.GiaGiam >= 0 ?
                        (decimal)(o.OrderDetails.product.GiaBan - ((o.OrderDetails.product.GiaBan * o.OrderDetails.product.GiaGiam) / 100)) :
                        (decimal)o.OrderDetails.product.GiaBan,
                }).ToList(),

                TongCong = orderInfo.Order.ctdh != null
                    ? (decimal)orderInfo.Order.ctdh.Sum(ct => ct.gia)
                    : 0
            }).ToList();
            return View(orderDtos);
        }

        private IActionResult GetOrdersByStatus(int? page, string searchName, string orderStatus)
        {
            try
            {
                // Lấy thông tin khách hàng đang đăng nhập từ HttpContext
                var loggedInCustomerClaim = HttpContext.User.FindFirst(ClaimTypes.Name);
                if (loggedInCustomerClaim != null)
                {
                    string loggedInCustomerEmail = loggedInCustomerClaim.Value;

                    // Tìm khách hàng theo email
                    var loggedInCustomer = _context.Customer
                        .Include(c => c.CustomerDetail)
                        .FirstOrDefault(c => c.Email == loggedInCustomerEmail);

                    if (loggedInCustomer != null)
                    {
                        var pageNumber = page ?? 1;
                        int pageSize = int.MaxValue;

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

                        var sortedProducts = productsQuery.ToList();

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
                // Xử lý exception theo nhu cầu của bạn, ví dụ: logging
                ViewBag.ErrorMessage = "Có lỗi xảy ra khi xử lý yêu cầu.";
                return View();
            }
        }

        //Hủy Đơn Hàng
        public IActionResult CancelOrder(int Id)
        {
            var order = _context.Order.Include(o => o.ctdh).FirstOrDefault(o => o.id == Id);

            if (order != null)
            {
                // Kiểm tra quyền truy cập của người dùng, ví dụ chỉ cho phép khách hàng hủy đơn
                if (User.IsInRole("Customer"))
                {
                    // Duyệt qua các mục chi tiết của đơn hàng để khôi phục số lượng sản phẩm trong kho hàng
                    foreach (var orderDetail in order.ctdh)
                    {
                        var inventoryItem = _context.Inventory.FirstOrDefault(i => i.ProductId == orderDetail.ProductId);
                        if (inventoryItem != null)
                        {
                            inventoryItem.SoLuong += orderDetail.soLuong;
                        }
                    }

                    // Cập nhật trạng thái đơn hàng là đã hủy
                    order.trangThai = "Đã hủy";
                    _context.SaveChanges();

                    TempData["SuccessMessage"] = "Đã hủy đơn hàng thành công.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Bạn không có quyền hủy đơn hàng.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn hàng.";
            }

            return RedirectToAction("CancelOrders"); // Chuyển hướng về trang danh sách đơn hàng
        }

        //Xác nhận Đơn Hàng
        public IActionResult ConfirmOrder(int Id)
        {
            var order = _context.Order.Find(Id);

            if (order != null)
            {
                // Kiểm tra quyền truy cập của người dùng, ví dụ chỉ cho phép khách hàng xác nhận đơn hàng
                if (User.IsInRole("Customer"))
                {
                    order.trangThai = "Hoàn thành";
                    _context.SaveChanges();

                    TempData["SuccessMessage"] = "Xác nhận và hoàn thành đơn hàng thành công.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Bạn không có quyền xác nhận đơn hàng.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn hàng.";
            }

            return RedirectToAction("Complete"); // Chuyển hướng về trang danh sách đơn hàng
        }
    }
}
