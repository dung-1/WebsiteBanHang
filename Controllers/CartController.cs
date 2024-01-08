using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Web.Helpers;
using WebsiteBanHang.Areas.Admin.Data;
using WebsiteBanHang.Areas.Admin.Models;



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


        public IActionResult Index()
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

                        // Lấy giỏ hàng của khách hàng
                        var cart = _context.CartModel
                            .Include(c => c.CartItems)
                            .ThenInclude(ci => ci.Product)
                            .FirstOrDefault(c => c.CustomerId == customerId);

                        if (cart != null)
                        {
                            return View(cart.CartItems);
                        }
                    }
                }

                // Xử lý khi không tìm thấy thông tin đăng nhập hoặc thông tin khách hàng (nếu cần)
                return View(new List<Cart_Item>()); // Trả về view với danh sách rỗng nếu không có thông tin đăng nhập hoặc giỏ hàng
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ (nếu cần)
                return RedirectToAction("Error"); // Chẳng hạn, chuyển hướng đến trang lỗi nếu có lỗi xảy ra
            }
        }

        public int GetCartItemCount()
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

                        // Lấy giỏ hàng của khách hàng
                        var cart = _context.CartModel
                            .Include(c => c.CartItems)
                            .FirstOrDefault(c => c.CustomerId == customerId);

                        if (cart != null)
                        {
                            // Tính tổng số lượng sản phẩm trong giỏ hàng
                            int totalQuantity = cart.CartItems.Sum(ci => ci.Quantity);

                            return totalQuantity;
                        }
                    }
                }

                // Trả về 0 nếu không tìm thấy thông tin đăng nhập hoặc giỏ hàng
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

                        // Lấy giỏ hàng của khách hàng
                        var cart = _context.CartModel
                            .Include(c => c.CartItems)
                                .ThenInclude(ci => ci.Product)
                            .FirstOrDefault(c => c.CustomerId == customerId);

                        if (cart != null)
                        {
                            // Tìm sản phẩm trong giỏ hàng
                            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Product.Id == productId);

                            if (cartItem != null)
                            {
                                // Cập nhật số lượng sản phẩm
                                if (quantity > 0)
                                {
                                    cartItem.Quantity = quantity;
                                }
                                else
                                {
                                    // Nếu số lượng là 0 hoặc âm, xóa sản phẩm khỏi giỏ hàng
                                    cart.CartItems.Remove(cartItem);
                                }

                                _context.SaveChanges();

                                // Trả về số lượng mới để cập nhật trên giao diện
                                return Json(new { success = true, newQuantity = cartItem.Quantity });
                            }
                        }
                    }
                }

                // Trả về lỗi nếu không tìm thấy thông tin đăng nhập hoặc giỏ hàng
                return Json(new { success = false, message = "Không thể cập nhật giỏ hàng." });
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ và trả về lỗi nếu có lỗi xảy ra
                return Json(new { success = false, message = "Đã xảy ra lỗi khi cập nhật giỏ hàng." });
            }
        }
        [HttpPost]
        public IActionResult RemoveFromCart(int id)
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

                        // Lấy giỏ hàng của khách hàng
                        var cart = _context.CartModel
                            .Include(c => c.CartItems)
                                .ThenInclude(ci => ci.Product)
                            .FirstOrDefault(c => c.CustomerId == customerId);

                        if (cart != null)
                        {
                            // Tìm và xóa sản phẩm khỏi giỏ hàng
                            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Product.Id == id);
                            if (cartItem != null)
                            {
                                cart.CartItems.Remove(cartItem);
                                _context.SaveChanges();

                                return Json(new { success = true });
                            }
                        }
                    }
                }

                // Trả về lỗi nếu không tìm thấy thông tin đăng nhập hoặc giỏ hàng
                return Json(new { success = false, message = "Không thể xóa sản phẩm khỏi giỏ hàng." });
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ và trả về lỗi nếu có lỗi xảy ra
                return Json(new { success = false, message = "Đã xảy ra lỗi khi xóa sản phẩm khỏi giỏ hàng." });
            }
        }






    }
}
