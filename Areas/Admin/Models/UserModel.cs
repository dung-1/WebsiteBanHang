using System.ComponentModel.DataAnnotations;

namespace WebsiteBanHang.Areas.Admin.Models
{
    public class UserModel :User
    {
        public ICollection<UserRoleModel> UserRole { get; } = new List<UserRoleModel>(); // Collection navigation containing dependents
        public ICollection<OrdersModel> Order { get; } = new List<OrdersModel>(); // Collection navigation containing dependents
        public Users_Details? userDetail { get; set; } = new Users_Details(); // Khởi tạo giá trị mặc định
        public int? ChatConnectionId { get; set; } // Foreign key for ChatConnection
        public ChatConnection? ChatConnection { get; set; }

    }
}
    