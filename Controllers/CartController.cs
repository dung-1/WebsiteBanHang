using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebsiteBanHang.Areas.Admin.Data;
using WebsiteBanHang.Areas.Admin.Models;
using WebsiteBanHang.HubSignalR;
using Microsoft.AspNetCore.SignalR;
using WebsiteBanHang.Models;
using Stripe.Checkout;
using Stripe;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace WebsiteBanHang.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CartController : Controller
    {
        private readonly IConfiguration _configuration;

        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHubContext<NotificationHub> _hub;
        private readonly StripeSettings _stripeSettings;
        public CartController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IHubContext<NotificationHub> hub, IOptions<StripeSettings> stripeSettings)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _hub = hub;
            _stripeSettings = stripeSettings.Value;
        }


        private (CustomerModel loggedInCustomer, CartModel cart) GetLoggedInCustomerAndCart()
        {
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
                    int customerId = loggedInCustomer.Id;

                    // Lấy giỏ hàng của khách hàng
                    var cart = _context.CartModel
                        .Include(c => c.CartItems)
                        .ThenInclude(ci => ci.Product)
                        .FirstOrDefault(c => c.CustomerId == customerId);

                    if (cart != null)
                    {
                        return (loggedInCustomer, cart);
                    }
                }
            }

            return (null, null);
        }

        public IActionResult Index()
        {
            try
            {
                var (loggedInCustomer, cart) = GetLoggedInCustomerAndCart();

                if (cart != null)
                {
                    return View(cart.CartItems);
                }

                return View(new List<Cart_Item>());
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ (nếu cần)
                return RedirectToAction("Error");
            }
        }

        public int GetCartItemCount()
        {
            try
            {
                var (_, cart) = GetLoggedInCustomerAndCart();

                if (cart != null)
                {
                    int totalQuantity = cart.CartItems.Sum(ci => ci.Quantity);
                    return totalQuantity;
                }

                return 0;
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ và trả về 0 nếu có lỗi xảy ra
                return 0;
            }
        }

        public IActionResult AddToCart(int id, int quantity)
        {
            try
            {
                // Kiểm tra xem người dùng đã đăng nhập chưa
                if (!User.Identity.IsAuthenticated)
                {
                    // Chuyển hướng người dùng đến trang đăng nhập
                    return RedirectToAction("Login", "Account");
                }

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
                        int customerId = loggedInCustomer.Id;

                        // Kiểm tra xem giỏ hàng của khách hàng đã tồn tại chưa
                        var cart = _context.CartModel.Include(c => c.CartItems).FirstOrDefault(c => c.CustomerId == customerId);

                        if (cart == null)
                        {
                            // Nếu giỏ hàng chưa tồn tại, tạo mới giỏ hàng
                            cart = new CartModel
                            {
                                CustomerId = customerId
                            };
                            _context.CartModel.Add(cart);
                            _context.SaveChanges(); // Lưu giỏ hàng mới vào cơ sở dữ liệu để nhận được giá trị Id mới
                        }

                        // Kiểm tra xem sản phẩm đã tồn tại trong giỏ hàng chưa
                        var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == id);

                        if (cartItem == null)
                        {
                            // Nếu sản phẩm chưa tồn tại trong giỏ hàng, thêm mới sản phẩm vào giỏ hàng với số lượng nhập vào
                            cartItem = new Cart_Item
                            {
                                CartId = cart.Id,
                                ProductId = id,
                                Quantity = quantity > 0 ? quantity : 1,
                            };
                            _context.Cart_Item.Add(cartItem);
                        }
                        else
                        {
                            // Nếu sản phẩm đã tồn tại trong giỏ hàng, tăng số lượng theo số lượng nhập vào
                            cartItem.Quantity += quantity;
                        }

                        _context.SaveChanges();

                        return RedirectToAction("Index");
                    }
                }

                // Xử lý khi không tìm thấy thông tin đăng nhập hoặc thông tin khách hàng (nếu cần)
                return RedirectToAction("Login", "Account"); // Chẳng hạn, chuyển hướng đến trang đăng nhập nếu không có thông tin đăng nhập
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ (nếu cần)
                return RedirectToAction("Error"); // Chẳng hạn, chuyển hướng đến trang lỗi nếu có lỗi xảy ra
            }
        }

        [HttpPost]
        public IActionResult UpdateCartItemQuantity(int productId, int quantity)
        {
            try
            {
                var (_, cart) = GetLoggedInCustomerAndCart();

                if (cart != null)
                {
                    var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Product.Id == productId);

                    if (cartItem != null)
                    {
                        if (quantity > 0)
                        {
                            cartItem.Quantity = quantity;
                        }
                        else
                        {
                            cart.CartItems.Remove(cartItem);
                        }

                        _context.SaveChanges();

                        return Json(new { success = true, newQuantity = cartItem.Quantity });
                    }
                }

                return Json(new { success = false, message = "Không thể cập nhật giỏ hàng." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi khi cập nhật giỏ hàng." });
            }
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int id)
        {
            try
            {
                var (_, cart) = GetLoggedInCustomerAndCart();

                if (cart != null)
                {
                    var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Product.Id == id);
                    if (cartItem != null)
                    {
                        cart.CartItems.Remove(cartItem);
                        _context.SaveChanges();
                        return Json(new { success = true });
                    }
                }

                return Json(new { success = false, message = "Không thể xóa sản phẩm khỏi giỏ hàng." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi khi xóa sản phẩm khỏi giỏ hàng." });
            }
        }
        private string GenerateOrderCode()
        {
            var latestOrder = _context.Order
                .OrderByDescending(order => order.MaHoaDon)
                .FirstOrDefault();

            if (latestOrder != null)
            {
                if (int.TryParse(latestOrder.MaHoaDon.Substring(2), out int latestOrderNumber))
                {
                    // Increment the order number and format it as HDxxxxx
                    return "HD" + (latestOrderNumber + 1).ToString("D5");
                }
            }
            return "HD00001";
        }


        public async Task<IActionResult> Checkout(string delivery_method)
        {
            var (loggedInCustomer, cartModel) = GetLoggedInCustomerAndCart();

            if (cartModel != null)
            {
                var cartItems = cartModel.CartItems;
                float total = 0;
                OrdersModel order = null;

                try
                {
                    if (delivery_method == "card")
                    {
                        order = new OrdersModel
                        {
                            MaHoaDon = GenerateOrderCode(),
                            CustomerID = loggedInCustomer?.Id,
                            ngayBan = DateTime.Now,
                            trangThai = "Đang xử lý",
                            LoaiHoaDon = "Thanh toán qua thẻ"
                        };

                        foreach (var cartItem in cartItems)
                        {
                            float price = CalculatePrice(cartItem);
                            total += price;

                            var orderDetail = new OrderDetaiModel
                            {
                                ProductId = cartItem.ProductId,
                                soLuong = cartItem.Quantity,
                                gia = price
                            };

                            var inventoryItem = _context.Inventory.FirstOrDefault(i => i.ProductId == cartItem.ProductId);
                            if (inventoryItem != null)
                            {
                                inventoryItem.SoLuong -= cartItem.Quantity;
                            }

                            order.ctdh.Add(orderDetail);
                        }

                        // Gán tổng tiền cho đơn hàng
                        order.tongTien = total;

                        // Thực hiện thanh toán qua Stripe
                        StripeConfiguration.ApiKey = _stripeSettings.SecretKey;

                        var options = new SessionCreateOptions
                        {
                            PaymentMethodTypes = new List<string> { "card" },
                            LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                Currency = "vnd",
                                UnitAmount = (int)total,  // Số tiền nhỏ nhất của đơn vị tiền tệ
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = "Product Name",
                                    Description = "Product Description"
                                }
                            },
                            Quantity = 1
                        }
                    },
                            Mode = "payment",
                            SuccessUrl = Url.Action("PaymentSuccess", "Cart", new { orderId = order.MaHoaDon }, Request.Scheme),
                            CancelUrl = Url.Action("PaymentCancel", "Cart", null, Request.Scheme)
                        };

                        var service = new SessionService();
                        var session = service.Create(options);

                        // Lưu thông tin đơn hàng vào session tạm thời để sử dụng sau khi thanh toán thành công
                        TempData["OrderInfo"] = JsonConvert.SerializeObject(order);

                        // Trả về URL để chuyển hướng người dùng đến trang thanh toán
                        return Json(new { success = true, stripeSessionUrl = session.Url });
                    }
                    else if (delivery_method == "cod")
                    {
                        // Thanh toán khi nhận hàng
                        order = new OrdersModel
                        {
                            MaHoaDon = GenerateOrderCode(),
                            CustomerID = loggedInCustomer?.Id,
                            ngayBan = DateTime.Now,
                            tongTien = total,
                            trangThai = "Đang xử lý",
                            LoaiHoaDon = "Thanh toán khi nhận hàng"
                        };

                        foreach (var cartItem in cartItems)
                        {
                            float price = CalculatePrice(cartItem);
                            total += price;

                            var orderDetail = new OrderDetaiModel
                            {
                                ProductId = cartItem.ProductId,
                                soLuong = cartItem.Quantity,
                                gia = price
                            };

                            var inventoryItem = _context.Inventory.FirstOrDefault(i => i.ProductId == cartItem.ProductId);
                            if (inventoryItem != null)
                            {
                                inventoryItem.SoLuong -= cartItem.Quantity;
                            }

                            order.ctdh.Add(orderDetail);
                        }

                        order.tongTien = total;
                        _context.Order.Add(order);
                        _context.SaveChanges();

                        _context.Cart_Item.RemoveRange(cartItems);
                        _context.SaveChanges();

                        await _hub.Clients.All.SendAsync("ReceiveOrderNotification", order.MaHoaDon);

                        return Json(new { success = true, redirectUrl = Url.Action("Index", "Cart") });
                    }
                }
                catch (Exception ex)
                {
                    TempData["CheckoutError"] = "Đã xảy ra lỗi khi đặt hàng.";
                    return RedirectToAction("Index", "Cart");
                }
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> PaymentSuccess(string orderId)
        {
            // Lấy thông tin đơn hàng từ TempData
            var orderInfo = TempData["OrderInfo"]?.ToString();
            if (orderInfo == null)
            {
                return RedirectToAction("Index", "Cart");
            }

            var order = JsonConvert.DeserializeObject<OrdersModel>(orderInfo);

            order.trangThai = "Đang xử lý";
            _context.Order.Add(order);
            _context.SaveChanges();

            var (loggedInCustomer, cartModel) = GetLoggedInCustomerAndCart();

            if (cartModel != null)
            {
                _context.Cart_Item.RemoveRange(cartModel.CartItems);
                _context.SaveChanges();
            }

            await _hub.Clients.All.SendAsync("ReceiveOrderNotification", order.MaHoaDon);

            return View();
        }


        public IActionResult PaymentCancel()
        {
            return View();
        }

        private float CalculatePrice(Cart_Item cartItem)
        {
            float price = 0;
            if (cartItem.Product.GiaGiam != 0)
            {
                price = (float)Convert.ToDouble(cartItem.Quantity * (cartItem.Product.GiaBan - (cartItem.Product.GiaBan * cartItem.Product.GiaGiam) / 100));
            }
            else
            {
                price = (float)Convert.ToDouble(cartItem.Quantity * cartItem.Product.GiaBan);
            }
            return price;
        }

    }
}
