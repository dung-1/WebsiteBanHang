using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebsiteBanHang.Areas.Admin.AdminDTO;
using WebsiteBanHang.Areas.Admin.Common;
using WebsiteBanHang.Areas.Admin.Controllers;
using WebsiteBanHang.Areas.Admin.Data;
using WebsiteBanHang.Areas.Admin.Models;
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
                                    join cancelOrder in _context.OrderCancel
                                    on order.id equals cancelOrder.OrderId into orderCancelDetails
                                    from orderCancel in orderCancelDetails.DefaultIfEmpty()

                                    where order.id == id && (orderCancel != null || orderCancel == null)// Lọc các đơn hàng bị hủy
                                    orderby order.id descending
                                    select new
                                    {
                                        Order = order,
                                        UserDetail = user,
                                        CustomerDetail = customer,
                                        OrderDetails = details,
                                        Product = orderProduct,
                                        OrderCancel = orderCancel
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

#pragma warning disable CS8601 // Possible null reference assignment.
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
                      img = o.Product.Image,
                      TenSanPham = o.Product.TenSanPham,
                      SoLuong = o.OrderDetails.soLuong,
                      Gia = o.Product.GiaGiam >= 0 ?
                          (decimal)(o.Product.GiaBan - ((o.Product.GiaBan * o.Product.GiaGiam) / 100)) :
                          (decimal)o.Product.GiaBan,
                  }).ToList(),

                HoaDonHuy = orderInfo.OrderCancel != null ? new HoaDonHuyDto
                {
                    Reason = orderInfo.OrderCancel.Reason,
                    ReasonAdmin = orderInfo.OrderCancel.AdminId != null
                  ? Enum.TryParse<CancelOfAdmin>(orderInfo.OrderCancel.Reason, out var reasonAdmin)
                    ? reasonAdmin
                    : (CancelOfAdmin?)null
                  : null,
                    ReasonCustomer = orderInfo.OrderCancel.CustomerId != null
                     ? Enum.TryParse<CancelOfClient>(orderInfo.OrderCancel.Reason, out var reasonCustomer)
                       ? reasonCustomer
                       : (CancelOfClient?)null
                     : null,
                    DateCancel = orderInfo.OrderCancel.CancelledAt,
                    UserCancel = orderInfo.OrderCancel.AdminId != null
                 ? "Hệ thống hủy đơn hàng"
                 : orderInfo.OrderCancel.CustomerId != null
                   ? "Khách hàng hủy đơn hàng"
                   : "Không xác định"
                } : null,


                TongCong = orderInfo.Order.ctdh != null
                      ? (decimal)orderInfo.Order.ctdh.Sum(ct => ct.gia)
                      : 0
            }).ToList();
#pragma warning restore CS8601 // Possible null reference assignment.

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
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> CancelOrderAsync([FromBody] OrderCancellationModel cancellationModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var order = await _context.Order.Include(o => o.Customer)
                    .Include(o => o.ctdh)
                    .ThenInclude(od => od.product)
                    .FirstOrDefaultAsync(o => o.id == cancellationModel.OrderId);

                if (order == null)
                {
                    return NotFound("Không tìm thấy đơn hàng.");
                }

                // Kiểm tra quyền truy cập của người dùng
                if (!User.IsInRole("Customer"))
                {
                    return Forbid("Bạn không có quyền hủy đơn hàng.");
                }

                // Cập nhật trạng thái đơn hàng là "Đã hủy"
                order.trangThai = "Đã hủy";

                // Lấy UserID của người đăng nhập vào hệ thống và gán cho trường UserID của đơn hàng
                var userName = User.FindFirstValue(ClaimTypes.Name);
                var user = await _context.Customer.FirstOrDefaultAsync(u => u.Email == userName);

                if (user == null)
                {
                    return BadRequest("Không tìm thấy người dùng với tên đăng nhập đã cung cấp.");
                }

                // Gán ID của người dùng cho UserID của đơn hàng
                order.CustomerID = user.Id;

                // Cập nhật lại số lượng hàng trong kho
                foreach (var orderDetail in order.ctdh)
                {
                    var inventoryItem = await _context.Inventory.FirstOrDefaultAsync(i => i.ProductId == orderDetail.ProductId);
                    if (inventoryItem != null)
                    {
                        inventoryItem.SoLuong += orderDetail.soLuong;
                    }
                }

                // Gán giá trị cho các trường cần thiết trong model hủy đơn hàng
                cancellationModel.CancelledAt = DateTime.Now;
                cancellationModel.CustomerId = user.Id;

                // Lưu thông tin vào bảng OrderCancellationModel
                _context.OrderCancel.Add(cancellationModel);
                await _context.SaveChangesAsync();


                return Ok(new { success = true, message = "Đã hủy đơn hàng thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Đã xảy ra lỗi khi hủy đơn hàng.", error = ex.Message });
            }
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

        [HttpGet]
        public IActionResult CancelReason(int id)
        {
            try
            {
                return PartialView("_CanCelCustomerReason");
            }
            catch
            {
                return View("~/Areas/Admin/Views/Shared/_ErrorAdmin.cshtml");
            }
        }
    }
}
