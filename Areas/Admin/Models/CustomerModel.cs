using System.ComponentModel.DataAnnotations;

namespace WebsiteBanHang.Areas.Admin.Models
{
    public class CustomerModel :User
    {
        public ICollection<CustomerRoleModel> CustomerRole { get; } = new List<CustomerRoleModel>(); // Collection navigation containing dependents
        public ICollection<OrdersModel> Order { get; } = new List<OrdersModel>(); // Collection navigation containing dependents
        public Customer_Details? CustomerDetail { get; set; } = new Customer_Details(); // Khởi tạo giá trị mặc định
        public ICollection<CartModel> Carts { get; } = new List<CartModel>();
        public string? ChatConnectionId { get; set; } // Foreign key for ChatConnection
        public ChatConnection? ChatConnection { get; set; }


    }
}
