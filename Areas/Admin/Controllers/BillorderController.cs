using MailKit.Security;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MimeKit.Text;
using MimeKit;
using System.Globalization;
using WebsiteBanHang.Areas.Admin.AdminDTO;
using WebsiteBanHang.Areas.Admin.Data;
using WebsiteBanHang.Areas.Admin.Models;
using X.PagedList;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using WebsiteBanHang.HubSignalR;
using WebsiteBanHang.Areas.Admin.Common;
using Newtonsoft.Json;


namespace WebsiteBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Employee")]
    public class BillorderController : Controller
    {
        private readonly IConfiguration _configuration;


        private readonly ILogger<BillorderController> _logger;

        private readonly ApplicationDbContext _context;
        private readonly IHubContext<NotificationHub> _hub;

        public BillorderController(ApplicationDbContext context, ILogger<BillorderController> logger, IConfiguration configuration, IHubContext<NotificationHub> hub)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
            _hub = hub;

        }

        private IPagedList<OrderDto> GetOrdersByStatus(string status, int? page, string searchName)
        {
            try
            {
                var pageNumber = page ?? 1; // Số trang mặc định (trang 1)
                int pageSize = int.MaxValue; // Số mục trên mỗi trang

                // Lấy danh sách đơn hàng kèm thông tin người dùng tương ứng
                var productsQuery = from order in _context.Order
                                    where order.trangThai == status
                                    orderby order.id descending
                                    select new
                                    {
                                        Order = order,
                                    };

                // Sắp xếp kết quả và chuyển đổi sang danh sách
                var sortedProducts = productsQuery.ToList();

                // Lưu trữ giá trị tìm kiếm để hiển thị lại trên giao diện người dùng
                ViewBag.SearchName = searchName ?? "";
                // Nếu searchName là null, gán giá trị mặc định

                // Chuyển đổi danh sách sang đối tượng PagedList
                IPagedList<OrderDto> pagedCategories = sortedProducts.Select(e => new OrderDto
                {
                    Id = e.Order.id,
                    MaHoaDon = e.Order.MaHoaDon,
                    NgayBan = e.Order.ngayBan,
                    LoaiHoaDon = e.Order.LoaiHoaDon,
                    TrangThai = e.Order.trangThai,
                }).ToPagedList(pageNumber, pageSize);

                return pagedCategories;
            }
            catch (Exception ex)
            {
                return (IPagedList<OrderDto>)View("~/Areas/Admin/Views/Shared/_ErrorAdmin.cshtml");

            }
        }


        public ActionResult Index(int? page, string searchName)
        {
            var pagedCategories = GetOrdersByStatus("Đang xử lý", page, searchName);
            return View(pagedCategories);
        }

        //public JsonResult GetAllOrders(int? page, string searchName)
        //{
        //    try
        //    {
        //        var pagedOrders = GetOrdersByStatus("Đang xử lý", page, searchName); // Pass null for page
        //        if (pagedOrders != null)
        //        {
        //            return Json(pagedOrders);
        //        }
        //        else
        //        {
        //            // Trả về lỗi 500 (Internal Server Error) nếu pagedOrders là null
        //            Response.StatusCode = 500;
        //            return Json(new { error = "Error retrieving orders" });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Xử lý các exception khác nếu cần
        //        Response.StatusCode = 500;
        //        return Json(new { error = "Error retrieving orders" });
        //    }
        //}

        public IActionResult Approved(int? page, string searchName)
        {
            var pagedCategories = GetOrdersByStatus("Đã duyệt", page, searchName);
            return View(pagedCategories);
        }

        public IActionResult Transport(int? page, string searchName)
        {
            var pagedCategories = GetOrdersByStatus("Đang giao hàng", page, searchName);
            return View(pagedCategories);
        }

        public IActionResult Complete(int? page, string searchName)
        {
            var pagedCategories = GetOrdersByStatus("Hoàn thành", page, searchName);
            return View(pagedCategories);
        }

        public IActionResult Failorder(int? page, string searchName)
        {
            var pagedCategories = GetOrdersByStatus("Đã hủy", page, searchName);
            return View(pagedCategories);
        }

        [HttpGet]
        public IActionResult View(int id)
        {
            try
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
                    : null
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

                return PartialView("_OrderView", orderDtos);
            }
            catch
            {
                return View("~/Areas/Admin/Views/Shared/_ErrorAdmin.cshtml");
            }
        }

        //Duyệt Đơn Hàng
        [HttpPost]
        public async Task<IActionResult> ApproveOrderAsync(int Id)
        {
            try
            {
                var order = _context.Order
               .Include(o => o.Customer)
               .Include(o => o.ctdh) // Load danh sách chi tiết đơn hàng
               .ThenInclude(od => od.product) // Load thông tin sản phẩm cho mỗi chi tiết đơn hàng
               .FirstOrDefault(o => o.id == Id);

                if (order != null)
                {
                    // Kiểm tra quyền truy cập của người dùng, ví dụ chỉ cho phép admin duyệt đơn
                    if (User.IsInRole("Admin") || User.IsInRole("Employee"))
                    {
                        // Cập nhật trạng thái đơn hàng là đã duyệt
                        order.trangThai = "Đã duyệt";

                        // Lấy UserID của người đăng nhập vào hệ thống và gán cho trường UserID của đơn hàng
                        // Lấy tên đăng nhập từ claim
                        var userName = User.FindFirstValue(ClaimTypes.Name);

                        // Tìm kiếm người dùng trong cơ sở dữ liệu dựa trên tên đăng nhập
                        var user = await _context.User.FirstOrDefaultAsync(u => u.Email == userName);

                        if (user != null)
                        {
                            // Gán ID của người dùng cho UserID của đơn hàng
                            order.UserID = user.Id;
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Không tìm thấy người dùng với tên đăng nhập đã cung cấp.";
                        }



                        // Lấy email của khách hàng
                        var customerEmail = order.Customer?.Email;

                        _context.SaveChanges();

                        TempData["SuccessMessage"] = "Đã duyệt đơn hàng thành công.";

                        // Gửi hóa đơn qua email
                        if (!string.IsNullOrEmpty(customerEmail))
                        {
                            SendInvoiceByEmail(customerEmail, order);
                            await _hub.Clients.All.SendAsync("ReceiveOrderNotification", order.MaHoaDon);
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Không tìm thấy email người nhận !!!";
                        }
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Bạn không có quyền duyệt đơn hàng.";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Không tìm thấy đơn hàng.";
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View("~/Areas/Admin/Views/Shared/_ErrorAdmin.cshtml");

            }

        }

        //sendmail
        private void SendInvoiceByEmail(string recipientEmail, OrdersModel order)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Nguyễn Văn Dụng", _configuration["EmailSettings:Email"]));
            emailMessage.To.Add(new MailboxAddress("Người Nhận", recipientEmail));
            emailMessage.Subject = "Hóa Đơn Mua Hàng";

            var builder = new BodyBuilder();

            // Bắt đầu tạo nội dung email
            builder.HtmlBody = "<div style='font-family: Arial, sans-serif; padding: 20px;'>";
            builder.HtmlBody += "<h1 style=\"color: #007bff; font-weight: bold; text-transform: uppercase;\">" +
                                "vifiretek <span style=\"font-size: 1.25rem; color: #dc3545;\">.VN</span></h1>";

            // Thêm tiêu đề hóa đơn
            builder.HtmlBody += "<h3 style='text-align: center; font-weight: bold;color: #007bff;'>HÓA ĐƠN MUA HÀNG CỦA BẠN</h3>";

            // Thêm thông tin hóa đơn
            builder.HtmlBody += "<p style=\"font-family: Arial, sans-serif; font-size: 16px;\">" +
                     $"<span style=\"float: left; width: 50%;\">Mã Hóa Đơn: {order.MaHoaDon}</span>" +
                     $"<br>" +
                     $"<span style=\"float: left; width: 50%;\">Hình thức thanh toán: {order.LoaiHoaDon}</span>" +
                     $"<span style=\"float: right; width: 50%; text-align: right;\">Ngày Mua: {order.ngayBan.ToString("dd/MM/yyyy HH:mm:ss")}</span>" +
                     $"<div style=\"clear: both;\"></div>" +
                     $"</p>";

            // Thêm bảng chi tiết đơn hàng
            builder.HtmlBody += "<table style='width:100%; border-collapse: collapse;'>";
            builder.HtmlBody += "<tr><th style='border: 1px solid #ddd; padding: 8px;'>STT</th><th style='border: 1px solid #ddd; padding: 8px;'>Sản phẩm</th><th style='border: 1px solid #ddd; padding: 8px;'>Số lượng</th><th style='border: 1px solid #ddd; padding: 8px;'>Đơn giá</th><th style='border: 1px solid #ddd; padding: 8px;'>Thành tiền</th></tr>";

            // Thêm chi tiết đơn hàng
            int stt = 1;
            decimal tongCong = 0;
            foreach (var orderDetail in order.ctdh)
            {
                // Sử dụng orderDetail để lấy thông tin sản phẩm từ đơn hàng
                var product = orderDetail.product;

                // Tính tổng tiền cho sản phẩm này
                decimal donGia = (decimal)(orderDetail.product.GiaGiam >= 0 ?
                    (decimal)(orderDetail.product.GiaBan - ((orderDetail.product.GiaBan * orderDetail.product.GiaGiam) / 100)) :
                    (decimal)orderDetail.product.GiaBan);
                decimal thanhTien = (decimal)(orderDetail.soLuong * donGia);
                builder.HtmlBody += $"<tr><td style='border: 1px solid #ddd; padding: 8px;text-align:center;'>{stt}</td><td style='border: 1px solid #ddd; padding: 8px;text-align:center;text-transform: capitalize;'>{product?.TenSanPham}</td><td style='border: 1px solid #ddd; padding: 8px;text-align:center;'>{orderDetail.soLuong}</td><td style='border: 1px solid #ddd; padding: 8px;text-align:center;'>{donGia.ToString("C0", new CultureInfo("vi-VN"))}</td><td style='border: 1px solid #ddd; padding: 8px;text-align:end;'>{thanhTien.ToString("C0", new CultureInfo("vi-VN"))}</td></tr>";
                tongCong += thanhTien;
                stt++;
            }


            // Kết thúc bảng
            builder.HtmlBody += "</table>";

            // Thêm tổng cộng
            builder.HtmlBody += $"<p style='text-align: right; font-weight: bold;font-site:18px;'>Tổng cộng: {tongCong.ToString("C0", new CultureInfo("vi-VN"))}</p>";

            // Thêm dòng chân trang và cảm ơn
            builder.HtmlBody += "<hr>";
            builder.HtmlBody += "<p style='text-align: center; font-weight: bold;'>Cảm ơn quý khách đã mua hàng của chúng tôi !!!</p>";

            // Kết thúc nội dung email
            builder.HtmlBody += "</div>";

            emailMessage.Body = new TextPart(TextFormat.Html)
            {
                Text = builder.HtmlBody
            };

            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(_configuration["EmailSettings:Email"], _configuration["EmailSettings:Password"]);
            smtp.Send(emailMessage);
            smtp.Disconnect(true);
        }

        //Giao Đơn Hàng
        public IActionResult DeliverOrder(int Id)
        {
            try
            {
                var order = _context.Order.Find(Id);

                if (order != null)
                {
                    // Kiểm tra quyền truy cập của người dùng, ví dụ chỉ cho phép admin giao hàng
                    if (User.IsInRole("Admin"))
                    {
                        // Cập nhật trạng thái đơn hàng là đã giao hàng
                        order.trangThai = "Đang giao hàng";
                        _context.SaveChanges();

                        TempData["SuccessMessage"] = "Đã giao hàng thành công.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Bạn không có quyền giao hàng.";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Không tìm thấy đơn hàng.";
                }

                return RedirectToAction("Approved"); // Chuyển hướng về trang danh sách đơn hàng
            }
            catch (Exception ex)
            {
                return View("~/Areas/Admin/Views/Shared/_ErrorAdmin.cshtml");


            }

        }

        //Hủy Đơn Hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
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
                if (!User.IsInRole("Admin") && !User.IsInRole("Employee"))
                {
                    return Forbid("Bạn không có quyền hủy đơn hàng.");
                }

                // Cập nhật trạng thái đơn hàng là "Đã hủy"
                order.trangThai = "Đã hủy";

                // Lấy UserID của người đăng nhập vào hệ thống và gán cho trường UserID của đơn hàng
                var userName = User.FindFirstValue(ClaimTypes.Name);
                var user = await _context.User.FirstOrDefaultAsync(u => u.Email == userName);

                if (user == null)
                {
                    return BadRequest("Không tìm thấy người dùng với tên đăng nhập đã cung cấp.");
                }

                // Gán ID của người dùng cho UserID của đơn hàng
                order.UserID = user.Id;

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
                cancellationModel.AdminId = user.Id;

                // Lưu thông tin vào bảng OrderCancellationModel
                _context.OrderCancel.Add(cancellationModel);
                await _context.SaveChangesAsync();

                // Gửi email thông báo hủy đơn hàng
                var customerEmail = order.Customer?.Email;
                if (!string.IsNullOrEmpty(customerEmail))
                {
                    SendCancellationEmail(customerEmail, order, cancellationModel);
                }

                return Ok(new { success = true, message = "Đã hủy đơn hàng thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Đã xảy ra lỗi khi hủy đơn hàng.", error = ex.Message });
            }
        }

        private void SendCancellationEmail(string recipientEmail, OrdersModel order, OrderCancellationModel cancellationModel)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Nguyễn Văn Dụng", _configuration["EmailSettings:Email"]));
            emailMessage.To.Add(new MailboxAddress("Người Nhận", recipientEmail));
            emailMessage.Subject = "Hóa Đơn Mua Hàng";

            var builder = new BodyBuilder();

            // Bắt đầu tạo nội dung email
            builder.HtmlBody = "<div style='font-family: Arial, sans-serif; padding: 20px;'>";
            builder.HtmlBody += "<h1 style=\"color: #007bff; font-weight: bold; text-transform: uppercase;\">" +
                                "vifiretek <span style=\"font-size: 1.25rem; color: #dc3545;\">.VN</span></h1>";

            // Thêm tiêu đề hóa đơn
            builder.HtmlBody += "<h3 style='text-align: center; font-weight: bold;color: #dc3545;'>ĐƠN HÀNG ĐÃ BỊ HỦY</h3>";

            // Thêm thông tin hóa đơn
            builder.HtmlBody += "<p style=\"font-family: Arial, sans-serif; font-size: 16px;\">" +
                     $"<span style=\"float: left; width: 50%;\">Mã Hóa Đơn: {order.MaHoaDon}</span>" +
                     $"<span style=\"float: right; width: 50%; text-align: right;\">Ngày Mua: {order.ngayBan.ToString("dd/MM/yyyy HH:mm:ss")}</span>" +
                     $"<div style=\"clear: both;\"></div>" +
                     $"</p>";

            // Thêm bảng chi tiết đơn hàng
            builder.HtmlBody += "<table style='width:100%; border-collapse: collapse;'>";
            builder.HtmlBody += "<tr><th style='border: 1px solid #ddd; padding: 8px;'>STT</th><th style='border: 1px solid #ddd; padding: 8px;'>Sản phẩm</th><th style='border: 1px solid #ddd; padding: 8px;'>Số lượng</th><th style='border: 1px solid #ddd; padding: 8px;'>Đơn giá</th><th style='border: 1px solid #ddd; padding: 8px;'>Thành tiền</th></tr>";

            // Thêm chi tiết đơn hàng
            int stt = 1;
            decimal tongCong = 0;
            foreach (var orderDetail in order.ctdh)
            {
                // Sử dụng orderDetail để lấy thông tin sản phẩm từ đơn hàng
                var product = orderDetail.product;

                // Tính tổng tiền cho sản phẩm này
                decimal donGia = (decimal)(orderDetail.product.GiaGiam >= 0 ?
                    (decimal)(orderDetail.product.GiaBan - ((orderDetail.product.GiaBan * orderDetail.product.GiaGiam) / 100)) :
                    (decimal)orderDetail.product.GiaBan);
                decimal thanhTien = (decimal)(orderDetail.soLuong * donGia);
                builder.HtmlBody += $"<tr><td style='border: 1px solid #ddd; padding: 8px;text-align:center;'>{stt}</td><td style='border: 1px solid #ddd; padding: 8px;text-align:center;text-transform: capitalize;'>{product?.TenSanPham}</td><td style='border: 1px solid #ddd; padding: 8px;text-align:center;'>{orderDetail.soLuong}</td><td style='border: 1px solid #ddd; padding: 8px;text-align:center;'>{donGia.ToString("C0", new CultureInfo("vi-VN"))}</td><td style='border: 1px solid #ddd; padding: 8px;text-align:end;'>{thanhTien.ToString("C0", new CultureInfo("vi-VN"))}</td></tr>";
                tongCong += thanhTien;
                stt++;
            }


            // Kết thúc bảng
            builder.HtmlBody += "</table>";

            // Thêm tổng cộng
            builder.HtmlBody += $"<p style='text-align: right; font-weight: bold;font-site:18px;'>Tổng cộng: {tongCong.ToString("C0", new CultureInfo("vi-VN"))}</p>";
            builder.HtmlBody += "<hr>";
            if (int.TryParse(cancellationModel.Reason, out int reasonInt))
            {
                var reasonEnum = (CancelOfAdmin)reasonInt;

                // Lấy Display Name của lý do hủy
                string reasonDisplayName = reasonEnum.GetDescription();

                builder.HtmlBody += $"<p>Lý do hủy: {reasonDisplayName}</p>";
            }

            builder.HtmlBody += $"<p>Thời gian hủy: {cancellationModel.CancelledAt:dd/MM/yyyy HH:mm:ss}</p>";
            // Thêm dòng chân trang và cảm ơn
            builder.HtmlBody += "<hr>";
            builder.HtmlBody += "<p style='text-align: center; font-weight: bold;'>Chúng tôi rất tiếc vì sự bất tiện này. !!!</p>";

            // Kết thúc nội dung email
            builder.HtmlBody += "</div>";

            emailMessage.Body = new TextPart(TextFormat.Html)
            {
                Text = builder.HtmlBody
            };

            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(_configuration["EmailSettings:Email"], _configuration["EmailSettings:Password"]);
            smtp.Send(emailMessage);
            smtp.Disconnect(true);
        }

        [HttpGet]
        public IActionResult CancelReason(int id)
        {
            try
            {
                return PartialView("_ViewCancelReason");
            }
            catch
            {
                return View("~/Areas/Admin/Views/Shared/_ErrorAdmin.cshtml");
            }
        }

    }

}
