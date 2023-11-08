using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MimeKit;
using MimeKit.Text;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WebsiteBanHang.Areas.Admin.Data;
using WebsiteBanHang.Areas.Admin.Models;
using WebsiteBanHang.Models;



namespace WebsiteBanHang.Controllers
{
    public class CartController : Controller
    {
        private readonly IConfiguration _configuration;

        private IMemoryCache _cache;
        private readonly ApplicationDbContext _context;
            private readonly IHttpContextAccessor _httpContextAccessor;
        public CartController(IMemoryCache cache, ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _cache = cache;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var cart = GetCartItems();
            return View(cart);
        }

        public IActionResult AddToCart(int id)
        {
            var product = _context.Product
                .Where(p => p.Id == id)
                .FirstOrDefault();
            if (product == null)
                return NotFound("Không có sản phẩm");

            var cart = GetCartItems();
            var cartItem = cart.Find(p => p.Product.Id == id);

            if (cartItem != null)
            {
                cartItem.Soluong++;
            }
            else
            {
                cart.Add(new CartItemModel() { Soluong = 1, Product = product });
            }

            SaveCartItems(cart);

            TempData["AddToCartSuccess"] = "Sản phẩm đã được thêm vào giỏ hàng thành công.";

            return RedirectToAction("Index","Home");
        }

        public IActionResult RemoveFromCart(int id)
        {
            var cart = GetCartItems();
            var cartItem = cart.Find(p => p.Product.Id == id);

            if (cartItem != null)
            {
                cart.Remove(cartItem);
                SaveCartItems(cart);

                TempData["DeleteSuccess"] = "Xóa sản phẩm khỏi giỏ hàng thành công.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateCartItem(int productId, int quantity)
        {
            var cart = GetCartItems();
            var cartItem = cart.Find(p => p.Product.Id == productId);

            if (cartItem != null)
            {
                cartItem.Soluong = quantity;
                SaveCartItems(cart);
            }

            return RedirectToAction("Index");
        }

        private List<CartItemModel> GetCartItems()
        {
            var cart = _cache.Get("cart");
            if (cart != null)
            {
                return JsonConvert.DeserializeObject<List<CartItemModel>>(cart.ToString());
            }
            return new List<CartItemModel>();
        }

        private void SaveCartItems(List<CartItemModel> cart)
        {
            var json = JsonConvert.SerializeObject(cart);
            _cache.Set("cart", json, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1) // Thời hạn lưu trữ của cookie
            });
        }

        public IActionResult Checkout(CheckOutModel model)
        {
            var cart = GetCartItems();
            string recipientEmail = model.Email;
            // Calculate the total price
            float totalPrice = (float)cart.Sum(cartItem => cartItem.Product.Gia * cartItem.Soluong);

            // Create a new order
            var order = new OrdersModel
            {
                MaHoaDon = GenerateOrderCode(), // Implement your own order code generation logic
                UserID = null,// Set the user ID as needed
                ngayBan = DateTime.Now,
                tongTien = totalPrice,
                trangThai = "Đang xử lý",
                LoaiHoaDon = "Mua hàng"
            };

            foreach (var cartItem in cart)
            {
                // Create an order detail for each item in the cart
                var orderDetail = new OrderDetaiModel
                {
                    ProductId = cartItem.Product.Id,
                    soLuong = cartItem.Soluong,
                    gia = (float)cartItem.Product.Gia
                };

                // Add the order detail to the order
                order.ctdh.Add(orderDetail);
            }

            // Save the order and order details to the database
            _context.Order.Add(order);
            _context.SaveChanges();
            SendInvoiceByEmail(recipientEmail, order);

            // Clear the cart after the purchase
            ClearCart();

            TempData["CheckoutSuccess"] = "Đặt hàng thành công.";

            return RedirectToAction("Index");
        }
        private void SendInvoiceByEmail(string recipientEmail, OrdersModel order)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Nguyễn Văn Dụng", _configuration["EmailSettings:Email"]));
            emailMessage.To.Add(new MailboxAddress("Người Nhận", recipientEmail));
            emailMessage.Subject = "Hóa Đơn Mua Hàng";

            var builder = new BodyBuilder();

            // Bắt đầu tạo nội dung email
            builder.HtmlBody = "<div style='font-family: Arial, sans-serif; padding: 20px;'>";

            // Thêm tiêu đề hóa đơn
            builder.HtmlBody += "<h2>HÓA ĐƠN CỦA BẠN</h2>";

            // Thêm thông tin hóa đơn
            builder.HtmlBody += $"<p>Mã Hóa Đơn: {order.MaHoaDon} | Ngày Mua: {order.ngayBan:dd/MM/yyyy HH:mm:ss}</p>";

            // Thêm bảng chi tiết đơn hàng
            builder.HtmlBody += "<table style='width:100%; border-collapse: collapse;'>";
            builder.HtmlBody += "<tr><th style='border: 1px solid #ddd; padding: 8px;'>STT</th><th style='border: 1px solid #ddd; padding: 8px;'>Sản phẩm</th><th style='border: 1px solid #ddd; padding: 8px;'>Số lượng</th><th style='border: 1px solid #ddd; padding: 8px;'>Đơn giá</th><th style='border: 1px solid #ddd; padding: 8px;'>Thành tiền</th></tr>";

            // Thêm chi tiết đơn hàng
            int stt = 1;
            decimal tongCong = 0;
            foreach (var orderDetail in order.ctdh)
            {
                var product = _context.Product.Find(orderDetail.ProductId);

                // Tính tổng tiền cho sản phẩm này
                decimal thanhTien = (decimal)(orderDetail.soLuong * orderDetail.gia);

                builder.HtmlBody += $"<tr><td style='border: 1px solid #ddd; padding: 8px;'>{stt}</td><td style='border: 1px solid #ddd; padding: 8px;'>{product?.TenSanPham}</td><td style='border: 1px solid #ddd; padding: 8px;'>{orderDetail.soLuong}</td><td style='border: 1px solid #ddd; padding: 8px;'>{orderDetail.gia.ToString("C0", new CultureInfo("vi-VN"))}</td><td style='border: 1px solid #ddd; padding: 8px;'>{thanhTien.ToString("C0", new CultureInfo("vi-VN"))}</td></tr>";

                tongCong += thanhTien;
                stt++;
            }

            // Kết thúc bảng
            builder.HtmlBody += "</table>";

            // Thêm tổng cộng
            builder.HtmlBody += $"<p style='text-align: right; font-weight: bold;'>Tổng cộng: {tongCong.ToString("C0", new CultureInfo("vi-VN"))}</p>";

            // Thêm dòng chân trang và cảm ơn
            builder.HtmlBody += "<hr>";
            builder.HtmlBody += "<p style='text-align: center; font-weight: bold;'>Cảm ơn bạn đã thanh toán !!!</p>";

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

        private void ClearCart()
        {
            // Remove the cart cookie
            Response.Cookies.Delete("cart");

            // Clear the in-memory cache if you're using it
            _cache.Remove("cart");
        }


        private string GenerateOrderCode()
        {
            // Get the latest order code from the database
            var latestOrder = _context.Order
                .OrderByDescending(order => order.MaHoaDon)
                .FirstOrDefault();

            if (latestOrder != null)
            {
                // Extract the numeric part of the latest order code
                if (int.TryParse(latestOrder.MaHoaDon.Substring(2), out int latestOrderNumber))
                {
                    // Increment the order number and format it as HDxxxxx
                    return "HD" + (latestOrderNumber + 1).ToString("D5");
                }
            }

            // If no existing orders, start from HD00001
            return "HD00001";
        }
        public IActionResult Checkoutcart()
        {
            return PartialView("CheckOutModal");
        }

        public IActionResult GetCartItemCount()
        {
            var cart = GetCartItems();
            var cartItemCount = cart.Sum(item => item.Soluong);
            return Json(cartItemCount);
        }


    }
}
