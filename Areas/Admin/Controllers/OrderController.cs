using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteBanHang.Areas.Admin.AdminDTO;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebsiteBanHang.Areas.Admin.Data;
using X.PagedList;

namespace WebsiteBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly ILogger<OrderController> _logger;

        private readonly ApplicationDbContext _context;
        public OrderController(ApplicationDbContext context, ILogger<OrderController> logger)
        {
            _context = context;
            _logger = logger;
        }
        [Authorize(Roles = "Admin,Employee")]

        public IActionResult Index(int? page, string searchName)
        {
            try
            {
                var pageNumber = page ?? 1; // Số trang mặc định (trang 1)
                int pageSize = 3; // Số mục trên mỗi trang

                // Lấy danh sách đơn hàng kèm thông tin người dùng tương ứng
                var productsQuery = from order in _context.Order
                                    join userDetail in _context.Users_Details
                                    on order.UserID equals userDetail.UserId into orderUserDetails
                                    from userDetail in orderUserDetails.DefaultIfEmpty()
                                    orderby order.id descending
                                    select new
                                    {
                                        Order = order,
                                        UserDetail = userDetail
                                    };

                // Áp dụng bộ lọc theo tên đơn hàng nếu có
                if (!string.IsNullOrEmpty(searchName))
                {
                    productsQuery = productsQuery.Where(p => p.Order.trangThai.Contains(searchName));
                }

                // Sắp xếp kết quả và chuyển đổi sang danh sách
                var sortedProducts = productsQuery.ToList();

                // Lưu trữ giá trị tìm kiếm để hiển thị lại trên giao diện người dùng
                ViewBag.SearchName = searchName ?? ""; // Nếu searchName là null, gán giá trị mặc định

                // Chuyển đổi danh sách sang đối tượng PagedList
                IPagedList<OrderDto> pagedCategories = sortedProducts.Select(e => new OrderDto
                {
                    Id = e.Order.id,
                    MaHoaDon = e.Order.MaHoaDon,
                    TenNhanVien = e.UserDetail != null ? e.UserDetail.HoTen : null,
                    NgayBan = e.Order.ngayBan,
                    LoaiHoaDon = e.Order.LoaiHoaDon,
                    TrangThai = e.Order.trangThai,
                }).ToPagedList(pageNumber, pageSize);

                // Hiển thị thông báo thành công nếu có
                if (TempData["SuccessMessage"] != null)
                {
                    ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
                }

                return View(pagedCategories);
            }
            catch (Exception ex)
            {
                // Xử lý exception theo nhu cầu của bạn, ví dụ: logging
                ViewBag.ErrorMessage = "Có lỗi xảy ra khi xử lý yêu cầu.";
                return View();
            }
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var order = _context.Order.FirstOrDefault(o => o.id == id);

            if (order == null)
            {
                return NotFound();
            }

            // Lấy danh sách Users_Details
            var userDetailList = _context.Users_Details.ToList();

            // Tạo SelectList để sử dụng trong dropdown với giá trị mặc định "-"
            var userDetailSelectList = new List<SelectListItem>
                    {
                        new SelectListItem { Value = null, Text =null } // Mục mặc định
                    };
            userDetailSelectList.AddRange(userDetailList.Select(ud => new SelectListItem
            {
                Value = ud.UserId.ToString(),
                Text = ud.HoTen,
            }));

            ViewBag.UserDetailList = new SelectList(userDetailSelectList, "Value", "Text");

            return PartialView("_OrderEdit", order);
        }
        public IActionResult Delete(int? id)
        {
            var deleterecord = _context.Order.Find(id);
            if (deleterecord == null)
            {
                return NotFound();
            }
            _context.Order.Remove(deleterecord);
            _context.SaveChanges();
            return RedirectToAction("Index");
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
                SoDienThoai = orderInfo.UserDetail?.SoDienThoai,
                DiaChi = orderInfo.UserDetail?.DiaChi,
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
                        Gia = (decimal)o.OrderDetails.gia,
                    }).ToList(),
                TongCong = orderInfo.Order.ctdh != null
                    ? (decimal)orderInfo.Order.ctdh.Sum(ct => ct.gia * ct.soLuong)
                    : 0
            }).ToList();

            return PartialView("_OrderView", orderDtos);
        }





    }
}
