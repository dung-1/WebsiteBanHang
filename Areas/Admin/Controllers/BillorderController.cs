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

namespace WebsiteBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Employee")]
    public class BillorderController : Controller
    {
        private readonly IConfiguration _configuration;


        private readonly ILogger<BillorderController> _logger;

        private readonly ApplicationDbContext _context;
        public BillorderController(ApplicationDbContext context, ILogger<BillorderController> logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        private IPagedList<OrderDto> GetOrdersByStatus(string status, int? page, string searchName)
        {
            try
            {
                var pageNumber = page ?? 1; // Số trang mặc định (trang 1)
                int pageSize = 10; // Số mục trên mỗi trang

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
                ViewBag.SearchName = searchName ?? ""; // Nếu searchName là null, gán giá trị mặc định

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
                // Xử lý exception theo nhu cầu của bạn, ví dụ: logging
                ViewBag.ErrorMessage = "Có lỗi xảy ra khi xử lý yêu cầu.";
                return null; // hoặc throw ex; tùy vào cách xử lý lỗi của bạn
            }
        }

        public IActionResult Index(int? page, string searchName)
        {
            var pagedCategories = GetOrdersByStatus("Đang xử lý", page, searchName);
            return View(pagedCategories);
        }

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

            return PartialView("_OrderView", orderDtos);
        }
        //Duyệt Đơn Hàng
        public IActionResult ApproveOrder(int Id)
        {
            var order = _context.Order
                .Include(o => o.Customer)
                .Include(o => o.ctdh) // Load danh sách chi tiết đơn hàng
                .ThenInclude(od => od.product) // Load thông tin sản phẩm cho mỗi chi tiết đơn hàng
                .FirstOrDefault(o => o.id == Id);

            if (order != null)
            {
                // Kiểm tra quyền truy cập của người dùng, ví dụ chỉ cho phép admin duyệt đơn
                if (User.IsInRole("Admin"))
                {
                    // Cập nhật trạng thái đơn hàng là đã duyệt
                    order.trangThai = "Đã duyệt";

                    // Lấy email của khách hàng
                    var customerEmail = order.Customer?.Email;

                    _context.SaveChanges();

                    TempData["SuccessMessage"] = "Đã duyệt đơn hàng thành công.";

                    // Gửi hóa đơn qua email
                    if (!string.IsNullOrEmpty(customerEmail))
                    {
                        SendInvoiceByEmail(customerEmail, order);
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Không tìm thấy email ngừi nhận !!!";
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
            builder.HtmlBody += "<h3 style='text-align: center; font-weight: bold;'>HÓA ĐƠN CỦA BẠN</h3>";

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

        //Hủy Đơn Hàng
        public IActionResult CancelOrder(int Id)
        {
            var order = _context.Order.Include(o => o.ctdh).FirstOrDefault(o => o.id == Id);

            if (order != null)
            {
                // Kiểm tra quyền truy cập của người dùng, ví dụ chỉ cho phép khách hàng hủy đơn
                if (User.IsInRole("Admin"))
                {
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

            return RedirectToAction("Failorder"); // Chuyển hướng về trang danh sách đơn hàng
        }



    }
}
