using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Web.Helpers;
using WebsiteBanHang.Areas.Admin.Data;
using WebsiteBanHang.Areas.Admin.Models;
using WebsiteBanHang.Models;



namespace WebsiteBanHang.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CartController : Controller
    {
        private readonly IConfiguration _configuration;

        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CartController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
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

        public IActionResult AddToCart(int Id)
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
                        var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == Id);

                        if (cartItem == null)
                        {
                            // Nếu sản phẩm chưa tồn tại trong giỏ hàng, thêm mới sản phẩm vào giỏ hàng với số lượng 1
                            cartItem = new Cart_Item
                            {
                                CartId = cart.Id,
                                ProductId = Id,
                                Quantity = 1
                            };
                            _context.Cart_Item.Add(cartItem);
                        }
                        else
                        {
                            // Nếu sản phẩm đã tồn tại trong giỏ hàng, tăng số lượng thêm 1
                            cartItem.Quantity += 1;
                        }

                        _context.SaveChanges();

                        return RedirectToAction("Index");
                    }
                }

                // Xử lý khi không tìm thấy thông tin đăng nhập hoặc thông tin khách hàng (nếu cần)
                return RedirectToAction("Login"); // Chẳng hạn, chuyển hướng đến trang đăng nhập nếu không có thông tin đăng nhập
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

        //[Route("CheckOut")]
        //public IActionResult CheckOut()
        //{
        //    return View();
        //}
        [Route("CheckOut")]
        public IActionResult Checkout()
        {
            var (loggedInCustomer, cartModel) = GetLoggedInCustomerAndCart();

            if (cartModel != null)
            {
                var cartItems = cartModel.CartItems; // Lấy danh sách các mục giỏ hàng từ đối tượng cartModel
                float total = 0;
                try
                {
                    // Khai báo biến order ở đây để sử dụng trong vòng lặp
                    var order = new OrdersModel
                    {
                        MaHoaDon = GenerateOrderCode(), // Implement your own order code generation logic
                        CustomerID = loggedInCustomer != null ? loggedInCustomer.Id : null, // Set the user ID as needed
                        ngayBan = DateTime.Now,
                        tongTien = total,
                        trangThai = "Đang xử lý",
                        LoaiHoaDon = "Mua hàng"
                    };

                    foreach (var cartItem in cartItems)
                    {
                        float price = 0;
                        if (cartItem.Product.GiaGiam != 0)
                        {
                            var giamgia = cartItem.Product.GiaBan - ((cartItem.Product.GiaBan * cartItem.Product.GiaGiam) / 100);
                            price = (float)Convert.ToDouble(cartItem.Quantity * giamgia);
                        }
                        else
                        {
                            price = (float)Convert.ToDouble(cartItem.Quantity * cartItem.Product.GiaBan);
                        }
                        total += price;

                        // Create an order detail for each item in the cart
                        var orderDetail = new OrderDetaiModel
                        {
                            ProductId = cartItem.ProductId, // Sử dụng ProductId thay vì cartItem.Product.Id
                            soLuong = cartItem.Quantity,
                            gia = price // Sử dụng giá được tính toán ở trên
                        };

                        // Reduce the quantity of the product in the inventory
                        var inventoryItem = _context.Inventory.FirstOrDefault(i => i.ProductId == cartItem.ProductId);
                        if (inventoryItem != null)
                        {
                            inventoryItem.SoLuong -= cartItem.Quantity;
                        }
                        // Add the order detail to the order
                        order.ctdh.Add(orderDetail);
                    }

                    // Assign total value after calculating
                    order.tongTien = total;

                    // Save the order and order details to the database
                    _context.Order.Add(order);
                    _context.SaveChanges();

                    // Clear the cart after the purchase
                    _context.Cart_Item.RemoveRange(cartItems); // Xóa các mục giỏ hàng
                    _context.SaveChanges();

                    TempData["CheckoutSuccess"] = "Đặt hàng thành công.";
                }
                catch (Exception ex)
                {
                    TempData["CheckoutError"] = "Đã xảy ra lỗi khi đặt hàng.";
                    // Handle the exception
                }
            }

            return RedirectToAction("Index");
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

    }
}
